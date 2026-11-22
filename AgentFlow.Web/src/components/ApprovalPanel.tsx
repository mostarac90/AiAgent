import { useState } from 'react';
import { useSubmitApproval } from '../hooks/useInvestigations';
import type { TicketInvestigationResult } from '../types/investigation';
import { FindingsCard } from './FindingsCard';

interface ApprovalPanelProps {
  investigationId: string;
  findings: TicketInvestigationResult | null;
}

export function ApprovalPanel({
  investigationId,
  findings,
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
          Your approval is needed
          {findings && ` for ticket #${findings.ticketId}`}
        </h3>
        <p className="text-slate-300 mt-1 text-sm">
          The agent recommends escalating this ticket and has paused
          until you decide. Nothing has been changed on the ticket.
        </p>
      </div>

      {findings && (
        <div className="space-y-3">
          <div>
            <h4 className="text-sm font-semibold text-amber-300">
              Why this needs your approval
            </h4>
            <p className="mt-1">
              {findings.escalationReason ||
                'The agent did not give a specific reason. Review the findings below.'}
            </p>
          </div>

          {findings.proposedAction && (
            <div>
              <h4 className="text-sm font-semibold text-amber-300">
                If you approve
              </h4>
              <p className="mt-1">{findings.proposedAction}</p>
            </div>
          )}

          <FindingsCard findings={findings} />
        </div>
      )}

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
            Approve escalation
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
