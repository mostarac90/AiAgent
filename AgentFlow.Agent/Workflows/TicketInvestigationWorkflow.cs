
using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using AgentFlow.Agent.Workflows.Executors;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows;

public static class TicketInvestigationWorkflow
{
    public static Workflow Create(AgentRunner agent)
    {
        var investigation = new InvestigationExecutor(agent);
        var classification = new ClassificationExecutor();

        var approval = new ApprovalPendingExecutor();
        var report = new ReportExecutor();

        // The workflow is the following: 
        // 1. Investigate the ticket
        // 2. Classify the ticket
        // 3. If the ticket is high severity, send it for approval
        // 4. If the ticket is not high severity, generate a report
        
        return new WorkflowBuilder(investigation)
          .AddEdge(investigation, classification)

          .AddEdge<TicketInvestigationResult>(
                classification,
                approval,
                result => result is not null &&
                          result.RequiresEscalation)

            .AddEdge<TicketInvestigationResult>(
                classification,
                report,
                result => result is not null &&
                          !result.RequiresEscalation)

            .WithOutputFrom(approval)
            .WithOutputFrom(report)
            .Build();

    }
}
