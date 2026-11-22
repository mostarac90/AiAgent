using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class ClassificationExecutor(IInvestigationObserver? observer = null)
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

        observer?.OnFindings(investigation);
        observer?.OnStep(investigation.RequiresEscalation
            ? $"Classified as {investigation.Severity} severity: escalation recommended"
            : $"Classified as {investigation.Severity} severity: no escalation needed");

        return ValueTask.FromResult(investigation);
    }
}
