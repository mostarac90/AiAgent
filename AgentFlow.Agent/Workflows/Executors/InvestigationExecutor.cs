
using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class InvestigationExecutor(AgentRunner agent)
    : Executor<int, TicketInvestigationResult>("Investigation")
{
    public override async ValueTask<TicketInvestigationResult> HandleAsync(
        int ticketId,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Investigating ticket {ticketId}");

        TicketInvestigationResult result =
            await agent.InvestigateTicketAsync(
                ticketId,
                cancellationToken);

        return result;
    }
}
