using AgentFlow.Agent.Agents;

await using var agent = await TicketInvestigationAgent.CreateAsync();

Console.WriteLine("Connected to MCP server!");
Console.WriteLine("\nAvailable MCP tools:");
foreach (var tool in agent.Tools)
{
    Console.WriteLine($"Name: {tool.Name}");
    Console.WriteLine($"Description: {tool.Description}");
    Console.WriteLine("-------------------------");
}

int checkpointCount = 0;

try
{
    var result = await agent.RunAsync(
        1042,
        requestApproval: investigation =>
        {
            Console.WriteLine("\nAPPROVAL REQUIRED");
            Console.WriteLine($"Ticket: {investigation.TicketId}");
            Console.WriteLine($"Customer: {investigation.Customer}");
            Console.WriteLine($"Severity: {investigation.Severity}");
            Console.WriteLine($"Issue: {investigation.Issue}");
            Console.Write("Approve escalation? (yes/no): ");

            bool approved = string.Equals(
                Console.ReadLine()?.Trim(), "yes", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(approved);
        },
        onCheckpoint: _ => Console.WriteLine($"Checkpoint created: {++checkpointCount}"));

    Console.WriteLine("\nWORKFLOW COMPLETED");
    Console.WriteLine(result.Output);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}
