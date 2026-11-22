namespace AgentFlow.Agent.Models;

/// <summary>
/// Response sent back through the approval request port. Carries the
/// investigation along so the final report keeps the agent's findings.
/// </summary>
public sealed record ApprovalDecision(
    TicketInvestigationResult Investigation,
    bool Approved);
