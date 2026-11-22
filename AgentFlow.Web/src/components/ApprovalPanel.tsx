
import { useState } from 'react';
import { useSubmitApproval } from '../hooks/useInvestigations';

interface ApprovalPanelProps {
  investigationId: string;
}

export function ApprovalPanel({
  investigationId,
}: ApprovalPanelProps) {
  const approvalMutation = useSubmitApproval(investigationId);
  const [submitted, setSubmitted] = useState(false);

  async function handleDecision(approved: boolean) {
    try {
      await approvalMutation.mutateAsync(approved);
      setSubmitted(true);
    } catch (error) {
      console.error('Approval failed:', error);
    }
  }

  return (
    <div className="border border-amber-500/40 bg-amber-500/10 rounded-xl p-6 space-y-4">
      <div>
        <h3 className="text-xl font-semibold text-amber-400">
          Human approval required
        </h3>

        <p className="text-slate-300 mt-2">
          The AI agent requires your permission before
          continuing with the proposed action.
        </p>
      </div>

      {approvalMutation.isError && (
        <p className="text-red-400">
          Could not submit the decision. Please try again.
        </p>
      )}

      {submitted ? (
        <p className="text-green-400">
          Decision submitted. Updating investigation...
        </p>
      ) : (
        <div className="flex gap-3">
          <button
            onClick={() => handleDecision(true)}
            disabled={approvalMutation.isPending}
            className="bg-green-600 hover:bg-green-700 disabled:opacity-50 px-5 py-3 rounded-lg"
          >
            Approve
          </button>

          <button
            onClick={() => handleDecision(false)}
            disabled={approvalMutation.isPending}
            className="bg-red-600 hover:bg-red-700 disabled:opacity-50 px-5 py-3 rounded-lg"
          >
            Reject
          </button>
        </div>
      )}
    </div>
  );
}
