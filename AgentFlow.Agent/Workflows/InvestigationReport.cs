using AgentFlow.Agent.Models;

namespace AgentFlow.Agent.Workflows;

internal static class InvestigationReport
{
    public static string Format(
        TicketInvestigationResult investigation,
        string? outcome = null)
    {
        static string Bullets(IEnumerable<string> items) =>
            string.Join(Environment.NewLine, items.Select(i => $"- {i}"));

        var report = $"""
            TICKET INVESTIGATION REPORT

            Ticket: {investigation.TicketId}
            Customer: {investigation.Customer}
            Severity: {investigation.Severity}
            Issue: {investigation.Issue}

            Possible causes:
            {Bullets(investigation.PossibleCauses)}

            Recommended actions:
            {Bullets(investigation.RecommendedActions)}
            """;

        if (investigation.RequiresEscalation)
        {
            report += $"""


                Escalation reason: {investigation.EscalationReason}
                Proposed action: {investigation.ProposedAction}
                """;
        }

        if (outcome is not null)
        {
            report += $"""


                Outcome: {outcome}
                """;
        }

        return report;
    }
}
