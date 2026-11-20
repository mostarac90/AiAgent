namespace AgentFlow.McpServer.Services
{
    public class TicketService() : ITicketService
    {
        public string GetTicket(int ticketId)
        {
            if (ticketId <= 0)
            {
                return "Invalid ticket ID: must be greater than 0.";
            }
            return ticketId switch
            {
                1042 => """
                Ticket 1042
                Customer: IKEA
                Priority: High
                Issue: API returns HTTP 500
                """,
                1050 => """
                Ticket 1050
                Customer: Klarna 
                Priority: Medium
                Issue: Unable to register a new account
                """,
                _ => "Ticket not found."
            };
        }
        public string GetCustomer(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return "Invalid customer name: must not be empty.";
            }
            return customerName.Trim().ToLowerInvariant() switch
            {
                "klarna" => "Klarna: Enterprise customer, Standard SLA",
                "ikea" => "IKEA: Enterprise customer, Premium SLA",
                _ => "Customer not found."
            };
        }
        public string SearchKnowledgeBase(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Invalid search query: must not be empty.";
            }
            return query.Trim().ToLowerInvariant() switch
            {
                "http 500" => "server errors",
                "register" => "Account registration issues may be caused by invalid input or system errors.",
                _ => "No matching articles found."
            };
        }
    }   
}
