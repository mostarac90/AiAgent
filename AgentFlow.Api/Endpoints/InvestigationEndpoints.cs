using AgentFlow.Api.Models;
using AgentFlow.Api.Services;

namespace AgentFlow.Api.Endpoints;

public static class InvestigationEndpoints
{
    public static IEndpointRouteBuilder MapInvestigationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/investigations");

        group.MapPost("/validate", (CreateInvestigationRequest request) =>
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
        group.MapPost("/", (
            CreateInvestigationRequest request,
            InvestigationService investigations) =>
        {
            if (request.TicketId <= 0)
            {
                return Results.BadRequest(
                    "TicketId must be greater than zero.");
            }

            var session = investigations.Start(request.TicketId);

            return Results.Accepted(
                $"/api/investigations/{session.Id}",
                new
                {
                    session.Id,
                    Status = "Running"
                });
        });

        // List active investigations (running or waiting for approval)
        group.MapGet("/", (InvestigationService investigations) =>
            Results.Ok(investigations.GetActive()
                .Select(s => s.GetStatus())));

        // Get investigation status
        group.MapGet("/{id:guid}", (
            Guid id,
            InvestigationService investigations) =>
        {
            var session = investigations.Get(id);

            return session is null
                ? Results.NotFound()
                : Results.Ok(session.GetStatus());
        });

        // Approve or reject escalation
        group.MapPost("/{id:guid}/approval", (
            Guid id,
            ApprovalRequest request,
            InvestigationService investigations) =>
        {
            var session = investigations.Get(id);

            if (session is null)
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

        return app;
    }
}
