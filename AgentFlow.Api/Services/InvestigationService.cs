using System.Collections.Concurrent;

namespace AgentFlow.Api.Services;

public sealed class InvestigationService(AgentProvider agentProvider)
{
    private const int MaxListed = 20;

    private readonly ConcurrentDictionary<Guid, InvestigationSession> _sessions = new();

    /// <summary>Starts an investigation in the background and returns its session.</summary>
    public InvestigationSession Start(int ticketId)
    {
        var session = new InvestigationSession(ticketId);
        _sessions.TryAdd(session.Id, session);

        // Execute without blocking the HTTP response.
        _ = Task.Run(async () =>
        {
            try
            {
                var agent = await agentProvider.GetAsync();

                var result = await agent.RunAsync(
                    ticketId,
                    session.WaitForApprovalAsync,
                    observer: session);

                session.Complete(result);
            }
            catch (Exception ex)
            {
                session.Fail(ex);
            }
        });

        return session;
    }

    /// <summary>
    /// All active investigations plus the most recent finished ones, newest first.
    /// </summary>
    public IReadOnlyList<InvestigationSession> GetRecent()
    {
        var ordered = _sessions.Values
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        var active = ordered.Where(s => s.IsActive);
        var finished = ordered.Where(s => !s.IsActive).Take(MaxListed);

        return active.Concat(finished)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public InvestigationSession? Get(Guid id) =>
        _sessions.TryGetValue(id, out var session) ? session : null;
}
