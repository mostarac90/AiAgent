
namespace AgentFlow.McpServer.Services
{
    public interface ITicketService
    {
        string GetTicket(int ticketId);
        string GetCustomer(string customerName);
        string SearchKnowledgeBase(string query);
    }
}
