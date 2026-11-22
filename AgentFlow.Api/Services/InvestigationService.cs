using AgentFlow.Agent.Agents;
using System.Collections.Concurrent;

namespace AgentFlow.Api.Services;

public sealed class InvestigationService(TicketInvestigationAgent agent)
{
    private readonly ConcurrentDictionary<Guid, InvestigationSession> _sessions = new();

    /// <summary>Starts an investigation in the background and returns its session.</summary>
    public InvestigationSession Start(int ticketId)
    {
        var session = new InvestigationSession();
        _sessions.TryAdd(session.Id, session);

        // Execute without blocking the HTTP response.
        _ = Task.Run(async () =>
        {
            try
            {
                var result = await agent.RunAsync(
                    ticketId,
                    investigation => session.WaitForApprovalAsync(investigation));

                session.Complete(result);
            }
            catch (Exception ex)
            {
                session.Fail(ex);
            }
        });

        return session;
    }

    public InvestigationSession? Get(Guid id) =>
        _sessions.TryGetValue(id, out var session) ? session : null;
}
