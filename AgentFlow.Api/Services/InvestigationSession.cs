using AgentFlow.Agent.Agents;
using AgentFlow.Agent.Models;

namespace AgentFlow.Api.Services;

public sealed record InvestigationStep(string Message, DateTimeOffset Timestamp);

public sealed class InvestigationSession(int ticketId) : IInvestigationObserver
{
    private readonly object _sync = new();
    private readonly List<InvestigationStep> _steps = [];

    private TaskCompletionSource<bool>? _approval;

    private string _status = "Running";
    private TicketInvestigationResult? _findings;
    private string? _decision;
    private object? _result;
    private string? _error;
    private DateTimeOffset? _completedAt;

    public Guid Id { get; } = Guid.NewGuid();

    public int TicketId { get; } = ticketId;

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    /// <summary>True while the investigation is running or waiting for approval.</summary>
    public bool IsActive
    {
        get
        {
            lock (_sync)
            {
                return _status is "Running" or "WaitingForApproval";
            }
        }
    }

    public void OnStep(string message)
    {
        lock (_sync)
        {
            _steps.Add(new InvestigationStep(message, DateTimeOffset.UtcNow));
        }
    }

    public void OnFindings(TicketInvestigationResult investigation)
    {
        lock (_sync)
        {
            _findings = investigation;
        }
    }

    public Task<bool> WaitForApprovalAsync(
        TicketInvestigationResult investigation)
    {
        lock (_sync)
        {
            _approval = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            _findings = investigation;
            _status = "WaitingForApproval";
            _steps.Add(new InvestigationStep(
                "Waiting for human approval", DateTimeOffset.UtcNow));

            return _approval.Task;
        }
    }

    public bool TryRespond(bool approved)
    {
        lock (_sync)
        {
            if (_status != "WaitingForApproval" ||
                _approval is null)
            {
                return false;
            }

            if (!_approval.TrySetResult(approved))
                return false;

            _approval = null;
            _decision = approved ? "Approved" : "Rejected";
            _status = "Running";
            _steps.Add(new InvestigationStep(
                approved ? "Escalation approved" : "Escalation rejected",
                DateTimeOffset.UtcNow));

            return true;
        }
    }

    public void Complete(object? result)
    {
        lock (_sync)
        {
            _result = result;
            _status = "Completed";
            _completedAt = DateTimeOffset.UtcNow;
            _steps.Add(new InvestigationStep(
                "Investigation completed", _completedAt.Value));
        }
    }

    public void Fail(Exception exception)
    {
        lock (_sync)
        {
            _error = exception.Message;
            _status = "Failed";
            _completedAt = DateTimeOffset.UtcNow;
            _steps.Add(new InvestigationStep(
                "Investigation failed", _completedAt.Value));
        }
    }

    public object GetStatus()
    {
        lock (_sync)
        {
            return new
            {
                Id,
                TicketId,
                Status = _status,
                CreatedAt,
                CompletedAt = _completedAt,
                Steps = _steps.ToList(),
                Findings = _findings,
                Decision = _decision,
                Result = _result,
                Error = _error
            };
        }
    }
}
