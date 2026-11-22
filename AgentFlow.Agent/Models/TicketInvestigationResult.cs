namespace AgentFlow.Agent.Models
{
    public sealed class TicketInvestigationResult
    {
        public int TicketId { get; set; }

        public string Customer { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Issue { get; set; } = string.Empty;

        public List<string> PossibleCauses { get; set; } = [];

        public List<string> RecommendedActions { get; set; } = [];

        public bool RequiresEscalation { get; set; }

        /// <summary>Why a human needs to approve the escalation (empty when no escalation is needed).</summary>
        public string EscalationReason { get; set; } = string.Empty;

        /// <summary>What approving the escalation would mean (empty when no escalation is needed).</summary>
        public string ProposedAction { get; set; } = string.Empty;
    }
}
