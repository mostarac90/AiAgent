import { useActiveInvestigations } from '../hooks/useInvestigations';
import type { InvestigationStatus } from '../types/investigation';

interface ActiveInvestigationsProps {
  selectedId: string | null;
  onSelect: (id: string) => void;
}

const statusStyles: Record<InvestigationStatus, string> = {
  Running: 'bg-yellow-500/15 text-yellow-400',
  WaitingForApproval: 'bg-amber-500/15 text-amber-400',
  Completed: 'bg-green-500/15 text-green-400',
  Failed: 'bg-red-500/15 text-red-400',
};

const statusLabels: Record<InvestigationStatus, string> = {
  Running: 'Running',
  WaitingForApproval: 'Needs approval',
  Completed: 'Completed',
  Failed: 'Failed',
};

export function ActiveInvestigations({
  selectedId,
  onSelect,
}: ActiveInvestigationsProps) {
  const { data, isPending, isError } = useActiveInvestigations();

  return (
    <section className="bg-slate-900 rounded-xl p-6">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold">
          Active investigations
        </h2>
        {data && (
          <span className="text-sm text-slate-400">
            {data.length}
          </span>
        )}
      </div>

      {isPending && (
        <p className="text-slate-400">Loading...</p>
      )}

      {isError && (
        <p className="text-red-400">
          Failed to load active investigations.
        </p>
      )}

      {data && data.length === 0 && (
        <p className="text-slate-400">
          No investigations are running right now.
        </p>
      )}

      {data && data.length > 0 && (
        <ul className="space-y-2">
          {data.map((investigation) => {
            const selected = investigation.id === selectedId;
            const ticketId = investigation.approvalRequest?.ticketId;

            return (
              <li key={investigation.id}>
                <button
                  onClick={() => onSelect(investigation.id)}
                  className={`w-full text-left rounded-lg p-3 flex items-center justify-between gap-3 border transition-colors ${
                    selected
                      ? 'border-blue-500 bg-slate-800'
                      : 'border-slate-800 hover:bg-slate-800'
                  }`}
                >
                  <div className="min-w-0">
                    {ticketId !== undefined && (
                      <p className="font-medium">
                        Ticket #{ticketId}
                      </p>
                    )}
                    <p className="text-xs text-slate-400 truncate">
                      {investigation.id}
                    </p>
                  </div>

                  <span
                    className={`shrink-0 text-xs font-medium px-2.5 py-1 rounded-full ${statusStyles[investigation.status]}`}
                  >
                    {statusLabels[investigation.status]}
                  </span>
                </button>
              </li>
            );
          })}
        </ul>
      )}
    </section>
  );
}
