
namespace AgentFlow.Api.Investigations;

public sealed class InvestigationSession
{
    private readonly object _sync = new();

    private TaskCompletionSource<bool>? _approval;

    private string _status = "Running";
    private object? _approvalRequest;
    private object? _result;
    private string? _error;

    public Guid Id { get; } = Guid.NewGuid();

    public Task<bool> WaitForApprovalAsync(
        object investigation)
    {
        lock (_sync)
        {
            _approval = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            _approvalRequest = investigation;
            _status = "WaitingForApproval";

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

            _approvalRequest = null;
            _approval = null;
            _status = "Running";

            return true;
        }
    }

    public void Complete(object? result)
    {
        lock (_sync)
        {
            _result = result;
            _approvalRequest = null;
            _status = "Completed";
        }
    }

    public void Fail(Exception exception)
    {
        lock (_sync)
        {
            _error = exception.Message;
            _approvalRequest = null;
            _status = "Failed";
        }
    }

    public object GetStatus()
    {
        lock (_sync)
        {
            return new
            {
                Id,
                Status = _status,
                ApprovalRequest = _approvalRequest,
                Result = _result,
                Error = _error
            };
        }
    }
}
