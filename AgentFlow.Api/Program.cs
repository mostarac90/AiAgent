using AgentFlow.Agent.Agents;
using AgentFlow.Api.Endpoints;
using AgentFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(await TicketInvestigationAgent.CreateAsync());
builder.Services.AddSingleton<InvestigationService>();

var app = builder.Build();

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
