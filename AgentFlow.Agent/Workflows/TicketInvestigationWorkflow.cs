
using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Workflows.Executors;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows;

public static class TicketInvestigationWorkflow
{
    public static Workflow Create(AgentRunner agent)
    {
        var investigation = new InvestigationExecutor(agent);
        var classification = new ClassificationExecutor();

        return new WorkflowBuilder(investigation)
            .AddEdge(investigation, classification)
            .WithOutputFrom(classification)
            .Build();
    }
}
