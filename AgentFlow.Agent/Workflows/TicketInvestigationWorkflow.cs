using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using AgentFlow.Agent.Workflows.Executors;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows;

public static class TicketInvestigationWorkflow
{
    public static Workflow Create(
        AgentRunner agent,
        IInvestigationObserver? observer = null)
    {
        var investigation = new InvestigationExecutor(agent, observer);
        var classification = new ClassificationExecutor(observer);

        var approvalPort =
            RequestPort.Create<TicketInvestigationResult, ApprovalDecision>(
                "TicketApproval");

        var decision = new ApprovalDecisionExecutor(observer);
        var report = new ReportExecutor(observer);

        // The workflow is the following:
        // 1. Investigate the ticket
        // 2. Classify the ticket
        // 3. If escalation is recommended, ask a human for approval
        // 4. Otherwise, generate a report directly

        return new WorkflowBuilder(investigation)

            .AddEdge(investigation, classification)

            .AddEdge<TicketInvestigationResult>(
                classification,
                approvalPort,
                result => result.RequiresEscalation)

            .AddEdge<TicketInvestigationResult>(
                classification,
                report,
                result => !result.RequiresEscalation)

            .AddEdge(approvalPort, decision)

            .WithOutputFrom(decision)
            .WithOutputFrom(report)

            .Build();
    }
}
