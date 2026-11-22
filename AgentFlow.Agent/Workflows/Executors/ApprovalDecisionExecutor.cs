using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ApprovalDecisionExecutor(IInvestigationObserver? observer = null)
    : Executor<ApprovalDecision, string>(
        "ApprovalDecision")
{
    public override ValueTask<string> HandleAsync(
        ApprovalDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string outcome = decision.Approved
            ? "Escalation approved by a human. No ticket modification has been performed."
            : "Escalation rejected by a human. No further action required.";

        observer?.OnStep("Final report generated");

        return ValueTask.FromResult(
            InvestigationReport.Format(decision.Investigation, outcome));
    }
}
