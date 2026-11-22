
using AgentFlow.Agent.Agents;
using AgentFlow.Api.Investigations;
using AgentFlow.Api.Models;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(await TicketInvestigationAgent.CreateAsync());

builder.Services.AddSingleton<
    ConcurrentDictionary<Guid, InvestigationSession>>();

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

app.MapPost("/api/investigations/validate",
    (CreateInvestigationRequest request) =>
    {
        if (request.TicketId <= 0)
        {
            return Results.BadRequest(new
            {
                Error = "TicketId must be greater than zero."
            });
        }

        return Results.Ok(new
        {
            Valid = true,
            TicketId = request.TicketId,
            Message = "Request validated successfully."
        });
    });


// Start an investigation
app.MapPost("/api/investigations",
    (
        CreateInvestigationRequest request,
        TicketInvestigationAgent agent,
        ConcurrentDictionary<Guid, InvestigationSession> sessions) =>
    {
        if (request.TicketId <= 0)
        {
            return Results.BadRequest(
                "TicketId must be greater than zero.");
        }

        var session = new InvestigationSession();

        sessions.TryAdd(session.Id, session);

        // Execute without blocking the HTTP response.
        _ = Task.Run(async () =>
        {
            try
            {
                var result = await agent.RunAsync(
                    request.TicketId,
                    investigation =>
                        session.WaitForApprovalAsync(investigation));

                session.Complete(result);
            }
            catch (Exception ex)
            {
                session.Fail(ex);
            }
        });

        return Results.Accepted(
            $"/api/investigations/{session.Id}",
            new
            {
                session.Id,
                Status = "Running"
            });
    });

// Get investigation status
app.MapGet("/api/investigations/{id:guid}",
    (
        Guid id,
        ConcurrentDictionary<Guid, InvestigationSession> sessions) =>
    {
        if (!sessions.TryGetValue(id, out var session))
            return Results.NotFound();

        return Results.Ok(session.GetStatus());
    });

// Approve or reject escalation
app.MapPost("/api/investigations/{id:guid}/approval",
    (
        Guid id,
        ApprovalRequest request,
        ConcurrentDictionary<Guid, InvestigationSession> sessions) =>
    {
        if (!sessions.TryGetValue(id, out var session))
            return Results.NotFound();

        if (!session.TryRespond(request.Approved))
        {
            return Results.Conflict(
                "No approval is currently pending.");
        }

        return Results.Ok(new
        {
            Message = "Approval decision submitted.",
            Approved = request.Approved
        });
    });

app.Run();

