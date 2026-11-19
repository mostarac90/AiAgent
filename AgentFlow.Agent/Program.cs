using OpenAI.Chat;


var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if(string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var client = new ChatClient(
    model: "gpt-5-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
);

var response = await client.CompleteChatAsync(
    "Explain agentic AI in three sentences."
);

Console.WriteLine(response.Value.Content[0].Text);


Console.Read(); 