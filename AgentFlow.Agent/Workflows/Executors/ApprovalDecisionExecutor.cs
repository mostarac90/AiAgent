using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ApprovalDecisionExecutor()
    : Executor<bool, string>(
        "ApprovalDecision")
{
    public override ValueTask<string> HandleAsync(
        bool approved,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string message = approved
            ? "Escalation approved. No ticket modification has been performed."
            : "Escalation rejected. No further action required.";

        return ValueTask.FromResult(message);
    }
}
