import type {
  InvestigationStatus,
  InvestigationStep,
} from '../types/investigation';

interface ProgressTimelineProps {
  steps: InvestigationStep[];
  status: InvestigationStatus;
}

function formatTime(timestamp: string): string {
  return new Date(timestamp).toLocaleTimeString([], {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  });
}

function StepIcon({
  isLast,
  status,
}: {
  isLast: boolean;
  status: InvestigationStatus;
}) {
  if (isLast && status === 'WaitingForApproval') {
    return <span className="text-amber-400">⏸</span>;
  }
  if (isLast && status === 'Failed') {
    return <span className="text-red-400">✕</span>;
  }
  return <span className="text-green-400">✓</span>;
}

export function ProgressTimeline({ steps, status }: ProgressTimelineProps) {
  return (
    <div>
      <h3 className="font-semibold mb-2">Progress</h3>
      <ol className="space-y-1.5">
        {steps.map((step, i) => (
          <li key={i} className="flex items-center gap-3 text-sm">
            <span className="w-4 flex justify-center shrink-0">
              <StepIcon isLast={i === steps.length - 1} status={status} />
            </span>
            <span className="flex-1">{step.message}</span>
            <span className="text-xs text-slate-500 tabular-nums">
              {formatTime(step.timestamp)}
            </span>
          </li>
        ))}

        {status === 'Running' && (
          <li className="flex items-center gap-3 text-sm text-yellow-400">
            <span className="w-4 flex justify-center shrink-0">
              <span className="inline-block w-3 h-3 border-2 border-yellow-400 border-t-transparent rounded-full animate-spin" />
            </span>
            <span>Agent is working…</span>
          </li>
        )}
      </ol>
    </div>
  );
}
