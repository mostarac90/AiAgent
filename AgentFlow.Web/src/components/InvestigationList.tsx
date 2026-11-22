import { useInvestigationList } from '../hooks/useInvestigations';
import type { Investigation } from '../types/investigation';

interface InvestigationListProps {
  selectedId: string | null;
  onSelect: (id: string) => void;
}

function statusBadge(investigation: Investigation): {
  label: string;
  style: string;
} {
  switch (investigation.status) {
    case 'Running':
      return { label: 'Running', style: 'bg-yellow-500/15 text-yellow-400' };
    case 'WaitingForApproval':
      return { label: 'Needs approval', style: 'bg-amber-500/15 text-amber-400' };
    case 'Failed':
      return { label: 'Failed', style: 'bg-red-500/15 text-red-400' };
    case 'Completed':
      if (investigation.decision === 'Approved') {
        return { label: 'Escalated', style: 'bg-blue-500/15 text-blue-400' };
      }
      if (investigation.decision === 'Rejected') {
        return { label: 'Escalation rejected', style: 'bg-slate-500/15 text-slate-300' };
      }
      return { label: 'Completed', style: 'bg-green-500/15 text-green-400' };
  }
}

function summary(investigation: Investigation): string {
  const findings = investigation.findings;
  if (findings) {
    return `${findings.customer} · ${findings.issue}`;
  }
  const lastStep = investigation.steps.at(-1);
  return lastStep?.message ?? 'Starting…';
}

function InvestigationRow({
  investigation,
  selected,
  onSelect,
}: {
  investigation: Investigation;
  selected: boolean;
  onSelect: (id: string) => void;
}) {
  const badge = statusBadge(investigation);

  return (
    <li>
      <button
        onClick={() => onSelect(investigation.id)}
        className={`w-full text-left rounded-lg p-3 flex items-center justify-between gap-3 border transition-colors ${
          selected
            ? 'border-blue-500 bg-slate-800'
            : 'border-slate-800 hover:bg-slate-800'
        }`}
      >
        <div className="min-w-0">
          <p className="font-medium">Ticket #{investigation.ticketId}</p>
          <p className="text-xs text-slate-400 truncate">
            {summary(investigation)}
          </p>
        </div>

        <span
          className={`shrink-0 text-xs font-medium px-2.5 py-1 rounded-full ${badge.style}`}
        >
          {badge.label}
        </span>
      </button>
    </li>
  );
}

export function InvestigationList({
  selectedId,
  onSelect,
}: InvestigationListProps) {
  const { data, isPending, isError } = useInvestigationList();

  const active = (data ?? []).filter(
    (i) => i.status === 'Running' || i.status === 'WaitingForApproval'
  );
  const recent = (data ?? []).filter(
    (i) => i.status === 'Completed' || i.status === 'Failed'
  );

  return (
    <section className="bg-slate-900 rounded-xl p-6 space-y-6">
      <div>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">Active investigations</h2>
          {data && (
            <span className="text-sm text-slate-400">{active.length}</span>
          )}
        </div>

        {isPending && <p className="text-slate-400">Loading...</p>}

        {isError && (
          <p className="text-red-400">Failed to load investigations.</p>
        )}

        {data && active.length === 0 && (
          <p className="text-slate-400">
            No investigations are running right now.
          </p>
        )}

        {active.length > 0 && (
          <ul className="space-y-2">
            {active.map((investigation) => (
              <InvestigationRow
                key={investigation.id}
                investigation={investigation}
                selected={investigation.id === selectedId}
                onSelect={onSelect}
              />
            ))}
          </ul>
        )}
      </div>

      {recent.length > 0 && (
        <div>
          <h2 className="text-lg font-semibold mb-3 text-slate-300">
            Recent
          </h2>
          <ul className="space-y-2">
            {recent.map((investigation) => (
              <InvestigationRow
                key={investigation.id}
                investigation={investigation}
                selected={investigation.id === selectedId}
                onSelect={onSelect}
              />
            ))}
          </ul>
        </div>
      )}
    </section>
  );
}
