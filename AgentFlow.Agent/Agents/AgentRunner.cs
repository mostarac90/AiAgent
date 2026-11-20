using AgentFlow.Agent.Models;
using ModelContextProtocol.Client;
using OpenAI.Chat;
using System.Text.Json;

namespace AgentFlow.Agent.Agents
{
    public sealed class AgentRunner
    {
        private readonly ChatClient _chatClient;
        private readonly McpClient _mcpClient;
        private readonly ChatCompletionOptions _options;

        private const int MaxIterations = 10;

        private static readonly HashSet<string> AllowedTools =
        [
            "GetTicket",
            "GetCustomer",
            "SearchKnowledgeBase"
        ];

        public AgentRunner(
            ChatClient chatClient,
            McpClient mcpClient,
            ChatCompletionOptions options)
        {
            _chatClient = chatClient;
            _mcpClient = mcpClient;
            _options = options;
        }

        public async Task<TicketInvestigationResult> InvestigateTicketAsync(
            int ticketId,
            CancellationToken cancellationToken = default)
        {
            var prompt = $"""
                Investigate ticket {ticketId} using available MCP tools.

                Retrieve ticket details, customer information,
                and relevant knowledge-base articles.

                Return only JSON containing:
                ticketId, customer, severity, issue,
                possibleCauses, recommendedActions,
                requiresEscalation.
                """;

            string response = await RunAsync(prompt, cancellationToken);

            return JsonSerializer.Deserialize<TicketInvestigationResult>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
                ?? throw new InvalidOperationException(
                    "Failed to deserialize investigation result.");
        }

        public async Task<string> RunAsync(
            string userPrompt,
            CancellationToken cancellationToken = default)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are a support assistant. " +
                    "Investigate tickets using available MCP tools. " +
                    "Do not modify any tickets."),

                new UserChatMessage(userPrompt)
            };

            for (int round = 0; round < MaxIterations; round++)
            {
                Console.WriteLine($"Agent round: {round + 1}");

                ChatCompletion completion =
                    await _chatClient.CompleteChatAsync(
                        messages,
                        _options,
                        cancellationToken);

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        return string.Concat(
                            completion.Content.Select(x => x.Text));

                    case ChatFinishReason.ToolCalls:
                        messages.Add(
                            new AssistantChatMessage(completion));

                        foreach (var toolCall in completion.ToolCalls)
                        {
                            if (!AllowedTools.Contains(toolCall.FunctionName))
                            {
                                throw new InvalidOperationException(
                                    $"Tool {toolCall.FunctionName} is not allowed.");
                            }

                            var toolResult = await ExecuteMcpToolAsync(
                                toolCall,
                                cancellationToken);

                            // Feed the extracted MCP text back to the model.
                            messages.Add(
                                new ToolChatMessage(
                                    toolCall.Id,
                                    toolResult));
                        }

                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unexpected finish reason: {completion.FinishReason}");
                }
            }

            throw new InvalidOperationException(
                "Agent exceeded maximum execution rounds.");
        }

        private async Task<string> ExecuteMcpToolAsync(
            ChatToolCall toolCall,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Executing: {toolCall.FunctionName}");

            var arguments = JsonSerializer.Deserialize<
                Dictionary<string, object?>>(
                toolCall.FunctionArguments.ToString());

            if (arguments is null)
            {
                throw new InvalidOperationException(
                    "Tool arguments are invalid.");
            }

            // Execute the actual MCP server tool.
            var result = await _mcpClient.CallToolAsync(
                toolCall.FunctionName,
                arguments);

            var output = string.Join(
                Environment.NewLine,
                result.Content
                    .OfType<ModelContextProtocol.Protocol.TextContentBlock>()
                    .Select(x => x.Text));

            if (result.IsError == true)
            {
                output = $"Tool execution failed: {output}";
            }

            return output;
        }
    }
}
