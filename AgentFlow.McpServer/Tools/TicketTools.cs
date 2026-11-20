using AgentFlow.McpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AgentFlow.McpServer.Tools
{
    [McpServerToolType]
    public class TicketTools(ITicketService ticketService)
    {
        private readonly ITicketService _ticketService = ticketService;

      
        [McpServerTool(Name = "GetTicket")]
        [Description("Get a support ticket by its ID.")]
        public string GetTicket(
            [Description("The ticket ID")] int ticketId)
        {
            return _ticketService.GetTicket(ticketId);
        }

        [McpServerTool(Name = "GetCustomer")]
        [Description("Get information about a customer.")]
        public string GetCustomer(
            [Description("Customer name")] string name)
        {
            return _ticketService.GetCustomer(name);
        }

        [McpServerTool(Name = "SearchKnowledgeBase")]
        [Description("Search the support knowledge base.")]
        public string SearchKnowledgeBase(
            [Description("Search terms")] string query)
        {
          return _ticketService.SearchKnowledgeBase(query);
        }
    }
}
