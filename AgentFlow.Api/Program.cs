using AgentFlow.Agent.Agents;
using AgentFlow.Api.Endpoints;
using AgentFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(await TicketInvestigationAgent.CreateAsync());
builder.Services.AddSingleton<InvestigationService>();
builder.Services.AddOpenApi();

var app = builder.Build();

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
