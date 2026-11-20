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


Workflow workFlow = TicketInvestigationWorkflow.Create(agentRunner);

await using Run run = await InProcessExecution.RunAsync(workFlow, 1042);

foreach(WorkflowEvent e in run.NewEvents)
{

    if(e is ExecutorCompletedEvent completedEvent)
    {
        Console.WriteLine(
             $"Executor completed: {completedEvent.ExecutorId}");

    }
}

Console.Read();