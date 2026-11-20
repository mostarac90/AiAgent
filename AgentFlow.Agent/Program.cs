using AgentFlow.Agent.Models;
using AgentFlow.Agent.Workflows;
using Microsoft.Agents.AI.Workflows;
using ModelContextProtocol.Client;
using OpenAI.Chat;

Console.WriteLine("Hello, World!");

var serverPath = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "../../../../AgentFlow.McpServer/bin/Debug/net10.0/AgentFlow.McpServer.dll"
    ));

if (!File.Exists(serverPath))
{
    Console.WriteLine($"Server not found: {serverPath}");
    return;
}

var transport = new StdioClientTransport(
    new StdioClientTransportOptions
    {
        Name = "AgentFlow Ticket Server",
        Command = "dotnet",
        Arguments = [serverPath],
        InheritEnvironmentVariables = true
    });

await using var mcpClient =
    await McpClient.CreateAsync(transport);

Console.WriteLine("Connected to MCP server!");

var tools = await mcpClient.ListToolsAsync();

Console.WriteLine("\nAvailable MCP tools:");

foreach (var tool in tools)
{
    Console.WriteLine($"Name: {tool.Name}");
    Console.WriteLine($"Description: {tool.Description}");
    Console.WriteLine("-------------------------");
}

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "OPENAI_API_KEY is not configured.");
}

var chatClient = new ChatClient(
    model: "gpt-5-mini",
    apiKey: apiKey);

var options = new ChatCompletionOptions();


// Add tools to options

foreach(var tool in tools)
{
    var chatTool = AgentFlow.Agent.Services.McpToolAdapter.ConvertToChatTool(tool);
    options.Tools.Add(chatTool);
};

var agentRunner = new AgentFlow.Agent.Agents.AgentRunner(
    chatClient,
    mcpClient,
    options);


Workflow workflow =
    TicketInvestigationWorkflow.Create(agentRunner);

var checkpointManager = CheckpointManager.Default;

await using StreamingRun run =
    await InProcessExecution.RunStreamingAsync(
        workflow,
        1042,
        checkpointManager);

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    var checkpoints = new List<CheckpointInfo>();

    switch (evt)
    {
        case RequestInfoEvent requestEvent:

            if (!requestEvent.Request.TryGetDataAs<TicketInvestigationResult>(
                    out var investigation))
            {
                throw new InvalidOperationException(
                    "Unexpected approval request payload.");
            }

            Console.WriteLine("\nAPPROVAL REQUIRED");
            Console.WriteLine($"Ticket: {investigation.TicketId}");
            Console.WriteLine($"Customer: {investigation.Customer}");
            Console.WriteLine($"Severity: {investigation.Severity}");
            Console.WriteLine($"Issue: {investigation.Issue}");

            Console.Write("Approve escalation? (yes/no): ");

            string? input = Console.ReadLine();

            bool approved = string.Equals(
                input?.Trim(),
                "yes",
                StringComparison.OrdinalIgnoreCase);

            ExternalResponse response =
                requestEvent.Request.CreateResponse(approved);

            await run.SendResponseAsync(response);

            break;

        case WorkflowOutputEvent output:
            Console.WriteLine("\nWORKFLOW COMPLETED");
            Console.WriteLine(output.Data);
            return;

        case WorkflowErrorEvent error:
            Console.Error.WriteLine(
                error.Exception?.ToString()
                ?? "Unknown workflow error.");
            return;

        case ExecutorFailedEvent failed:
            Console.Error.WriteLine(
                $"Executor {failed.ExecutorId} failed: {failed.Data}");
            return;

        case SuperStepCompletedEvent completed:

            CheckpointInfo? checkpoint =
                completed.CompletionInfo?.Checkpoint;

            if (checkpoint is not null)
            {
                checkpoints.Add(checkpoint);

                Console.WriteLine(
                    $"Checkpoint created: {checkpoints.Count}");
            }

            break;
    }

}

Console.Read();