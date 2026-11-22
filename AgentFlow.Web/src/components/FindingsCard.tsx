import type { TicketInvestigationResult } from '../types/investigation';
import { SeverityBadge } from './SeverityBadge';

interface FindingsCardProps {
  findings: TicketInvestigationResult;
}

export function FindingsCard({ findings }: FindingsCardProps) {
  return (
    <div className="bg-slate-800 rounded-lg p-4 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="font-semibold">Agent findings</h3>
        <SeverityBadge severity={findings.severity} />
      </div>

      <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 text-sm">
        <dt className="text-slate-400">Customer</dt>
        <dd>{findings.customer}</dd>
        <dt className="text-slate-400">Issue</dt>
        <dd>{findings.issue}</dd>
      </dl>

      {findings.possibleCauses.length > 0 && (
        <div>
          <h4 className="text-sm font-medium text-slate-300 mb-1">
            Possible causes
          </h4>
          <ul className="list-disc list-inside text-sm space-y-0.5">
            {findings.possibleCauses.map((cause, i) => (
              <li key={i}>{cause}</li>
            ))}
          </ul>
        </div>
      )}

      {findings.recommendedActions.length > 0 && (
        <div>
          <h4 className="text-sm font-medium text-slate-300 mb-1">
            Recommended actions
          </h4>
          <ul className="list-disc list-inside text-sm space-y-0.5">
            {findings.recommendedActions.map((action, i) => (
              <li key={i}>{action}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
