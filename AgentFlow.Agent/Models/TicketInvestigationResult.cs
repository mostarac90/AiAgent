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
    }
}
