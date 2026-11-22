using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;
using Microsoft.Agents.AI.Workflows;

namespace AgentFlow.Agent.Workflows.Executors;

public sealed class InvestigationExecutor(
    AgentRunner agent,
    IInvestigationObserver? observer = null)
    : Executor<int, TicketInvestigationResult>("Investigation")
{
    public override async ValueTask<TicketInvestigationResult> HandleAsync(
        int ticketId,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Investigating ticket {ticketId}");
        observer?.OnStep($"Started investigating ticket #{ticketId}");

        TicketInvestigationResult result =
            await agent.InvestigateTicketAsync(
                ticketId,
                observer,
                cancellationToken);

        return result;
    }
}
