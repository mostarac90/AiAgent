using AgentFlow.Agent.Models;
using AgentFlow.Agent.Services;
using AgentFlow.Agent.Workflows;
using Microsoft.Agents.AI.Workflows;
using ModelContextProtocol.Client;
using OpenAI.Chat;

namespace AgentFlow.Agent.Agents;

public record WorkflowRunResult(bool Success, string? Output);

public sealed class TicketInvestigationAgentOptions
{
    public string? OpenAiApiKey { get; set; }
        = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    public string Model { get; set; } = "gpt-5-mini";

    /// <summary>Optional path to AgentFlow.McpServer.dll. Falls back to the bundled "mcp-server" folder, then the dev build.</summary>
    public string? McpServerPath { get; set; }
}

/// <summary>
/// Owns MCP connection, chat client and workflow execution.
/// Create once (e.g. as a singleton in the API) via <see cref="CreateAsync"/>.
/// </summary>
public sealed class TicketInvestigationAgent : IAsyncDisposable
{
    private readonly McpClient _mcpClient;
    private readonly AgentRunner _agentRunner;

    private TicketInvestigationAgent(McpClient mcpClient, AgentRunner agentRunner)
    {
        _mcpClient = mcpClient;
        _agentRunner = agentRunner;
    }

    public IReadOnlyList<McpClientTool> Tools { get; private init; } = [];

    public static async Task<TicketInvestigationAgent> CreateAsync(
        TicketInvestigationAgentOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new TicketInvestigationAgentOptions();

        if (string.IsNullOrWhiteSpace(options.OpenAiApiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY is not configured.");
        }

        var serverPath = ResolveMcpServerPath(options.McpServerPath);
        var transport = new StdioClientTransport(
            new StdioClientTransportOptions
            {
                Name = "AgentFlow Ticket Server",
                Command = "dotnet",
                Arguments = [serverPath],
                InheritEnvironmentVariables = true
            });

        var mcpClient = await McpClient.CreateAsync(
            transport, cancellationToken: cancellationToken);

        try
        {
            var tools = await mcpClient.ListToolsAsync(
                cancellationToken: cancellationToken);

            var chatOptions = new ChatCompletionOptions();
            foreach (var tool in tools)
            {
                chatOptions.Tools.Add(McpToolAdapter.ConvertToChatTool(tool));
            }

            var chatClient = new ChatClient(options.Model, options.OpenAiApiKey);
            var runner = new AgentRunner(chatClient, mcpClient, chatOptions);

            return new TicketInvestigationAgent(mcpClient, runner) { Tools = tools.ToList() };
        }
        catch
        {
            await mcpClient.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Finds AgentFlow.McpServer.dll: explicit/configured path, then the copy
    /// bundled next to the app (deployed), then the sibling project's Debug build (dev).
    /// </summary>
    private static string ResolveMcpServerPath(string? configuredPath)
    {
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            candidates.Add(Path.GetFullPath(configuredPath, AppContext.BaseDirectory));
        }

        candidates.Add(Path.Combine(
            AppContext.BaseDirectory, "mcp-server", "AgentFlow.McpServer.dll"));

        candidates.Add(Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../AgentFlow.McpServer/bin/Debug/net10.0/AgentFlow.McpServer.dll")));

        var found = candidates.FirstOrDefault(File.Exists);

        return found ?? throw new FileNotFoundException(
            "MCP server not found. Looked in: " + string.Join("; ", candidates));
    }

    /// <summary>
    /// Runs the investigation workflow. <paramref name="requestApproval"/> is awaited
    /// when human approval is needed (console prompt, API callback, ...).
    /// </summary>
    public async Task<WorkflowRunResult> RunAsync(
        int ticketId,
        Func<TicketInvestigationResult, Task<bool>> requestApproval,
        Action<CheckpointInfo>? onCheckpoint = null,
        IInvestigationObserver? observer = null,
        CancellationToken cancellationToken = default)
    {
        Workflow workflow = TicketInvestigationWorkflow.Create(_agentRunner, observer);

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            ticketId,
            CheckpointManager.Default,
            cancellationToken: cancellationToken);

        await foreach (WorkflowEvent evt in run.WatchStreamAsync(cancellationToken))
        {
            switch (evt)
            {
                case RequestInfoEvent requestEvent:
                    if (!requestEvent.Request.TryGetDataAs<TicketInvestigationResult>(
                            out var investigation))
                    {
                        throw new InvalidOperationException(
                            "Unexpected approval request payload.");
                    }

                    bool approved = await requestApproval(investigation);
                    await run.SendResponseAsync(
                        requestEvent.Request.CreateResponse(
                            new ApprovalDecision(investigation, approved)));
                    break;

                case WorkflowOutputEvent output:
                    return new WorkflowRunResult(true, output.Data?.ToString());

                case WorkflowErrorEvent error:
                    throw error.Exception ?? new InvalidOperationException("Workflow failed.");

                case ExecutorFailedEvent failed:
                    throw new InvalidOperationException(
                        $"Executor {failed.ExecutorId} failed: {failed.Data}");

                case SuperStepCompletedEvent completed:
                    if (completed.CompletionInfo?.Checkpoint is { } checkpoint)
                    {
                        onCheckpoint?.Invoke(checkpoint);
                    }
                    break;
            }
        }

        throw new InvalidOperationException("Workflow ended without output.");
    }

    public ValueTask DisposeAsync() => _mcpClient.DisposeAsync();
}
