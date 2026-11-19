namespace AgentFlow.Agent
{
    public static class TicketService
    {
        public static string GetTicket(int id)
        {
            return id switch
            {
                1042 => """
                Ticket: 1042
                Customer: Contoso
                Issue: API returns HTTP 500
                Priority: High
                Status: Open
                """,

                _ => "Ticket not found"
            };


        }
    }
}