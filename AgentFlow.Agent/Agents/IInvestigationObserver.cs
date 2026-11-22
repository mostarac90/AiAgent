using AgentFlow.Agent.Models;

namespace AgentFlow.Agent.Agents;

/// <summary>
/// Receives progress from a running investigation so a UI can show
/// what the agent is doing.
/// </summary>
public interface IInvestigationObserver
{
    /// <summary>A human-readable step, e.g. "Looking up customer IKEA".</summary>
    void OnStep(string message);

    /// <summary>The agent has finished investigating and produced its findings.</summary>
    void OnFindings(TicketInvestigationResult investigation);
}
