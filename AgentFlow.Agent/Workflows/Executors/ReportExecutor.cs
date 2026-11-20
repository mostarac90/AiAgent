
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ReportExecutor()
    : Executor<TicketInvestigationResult, string>(
        "Report")
{
    public override ValueTask<string> HandleAsync(
        TicketInvestigationResult investigation,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string report = $"""
            TICKET INVESTIGATION REPORT

            Ticket: {investigation.TicketId}
            Customer: {investigation.Customer}
            Severity: {investigation.Severity}
            Issue: {investigation.Issue}

            Recommended actions:
            {string.Join(
                Environment.NewLine,
                investigation.RecommendedActions)}
            """;

        return ValueTask.FromResult(report);
    }
}
