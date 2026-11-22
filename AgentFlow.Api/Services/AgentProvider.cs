using AgentFlow.Agent.Agents;

namespace AgentFlow.Api.Services;

/// <summary>
/// Creates the agent on first use instead of at startup, so a missing MCP server
/// or API key surfaces as an investigation error rather than crashing the app.
/// A failed creation is retried on the next call.
/// </summary>
public sealed class AgentProvider(TicketInvestigationAgentOptions options) : IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private TicketInvestigationAgent? _agent;

    public async Task<TicketInvestigationAgent> GetAsync(
        CancellationToken cancellationToken = default)
    {
        if (_agent is not null)
            return _agent;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            return _agent ??= await TicketInvestigationAgent.CreateAsync(
                options, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_agent is not null)
            await _agent.DisposeAsync();
    }
}
