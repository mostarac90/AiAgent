
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ApprovalPendingExecutor()
    : Executor<TicketInvestigationResult, string>(
        "ApprovalPending")
{
    public override ValueTask<string> HandleAsync(
        TicketInvestigationResult investigation,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string message = $"""
            APPROVAL REQUIRED

            Ticket: {investigation.TicketId}
            Customer: {investigation.Customer}
            Severity: {investigation.Severity}
            Issue: {investigation.Issue}

            Escalation has been recommended.
            No ticket modification has been performed.
            """;

        return ValueTask.FromResult(message);
    }
}
