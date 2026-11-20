
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ClassificationExecutor()
    : Executor<TicketInvestigationResult, TicketInvestigationResult>(
        "Classification")
{
    public override ValueTask<TicketInvestigationResult> HandleAsync(
        TicketInvestigationResult investigation,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            $"Ticket severity: {investigation.Severity}");

        Console.WriteLine(
            $"Requires escalation: {investigation.RequiresEscalation}");

        return ValueTask.FromResult(investigation);
    }
}
