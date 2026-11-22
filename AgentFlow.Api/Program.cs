using AgentFlow.Agent.Agents;
using AgentFlow.Api.Endpoints;
using AgentFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Config: OpenAI:ApiKey (or OPENAI_API_KEY), OpenAI:Model, McpServer:Path (optional).
var agentOptions = new TicketInvestigationAgentOptions();
if (builder.Configuration["OpenAI:ApiKey"] is { Length: > 0 } apiKey)
    agentOptions.OpenAiApiKey = apiKey;
if (builder.Configuration["OpenAI:Model"] is { Length: > 0 } model)
    agentOptions.Model = model;
agentOptions.McpServerPath = builder.Configuration["McpServer:Path"];

builder.Services.AddSingleton(agentOptions);
builder.Services.AddSingleton<AgentProvider>();
builder.Services.AddSingleton<InvestigationService>();
builder.Services.AddOpenApi();

var frontendUrl = builder.Configuration["FrontendUrl"];

if (string.IsNullOrWhiteSpace(frontendUrl))
{
    throw new InvalidOperationException(
        "FrontendUrl configuration is missing.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendUrl.TrimEnd('/'))
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AgentFlow API v1");
        options.RoutePrefix = "swagger";
    });
}

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Service = "AgentFlow.Api",
        Timestamp = DateTimeOffset.UtcNow
    });
});

app.MapInvestigationEndpoints();

app.Run();
