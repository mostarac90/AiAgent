using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ReportExecutor(IInvestigationObserver? observer = null)
    : Executor<TicketInvestigationResult, string>(
        "Report")
{
    public override ValueTask<string> HandleAsync(
        TicketInvestigationResult investigation,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        observer?.OnStep("Final report generated");

        return ValueTask.FromResult(
            InvestigationReport.Format(
                investigation,
                "No escalation needed. Report completed automatically."));
    }
}
