using ModelContextProtocol.Client;
using OpenAI.Chat;

namespace AgentFlow.Agent.Services;

public static class McpToolAdapter
{
    public static ChatTool ConvertToChatTool(
        McpClientTool mcpTool)
    {
        return ChatTool.CreateFunctionTool(
            functionName: mcpTool.Name,
            functionDescription: mcpTool.Description ?? "",
            functionParameters: BinaryData.FromString(
                mcpTool.JsonSchema.GetRawText())
        );
    }
}