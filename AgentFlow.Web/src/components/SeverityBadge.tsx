interface SeverityBadgeProps {
  severity: string;
}

function severityStyle(severity: string): string {
  switch (severity.trim().toLowerCase()) {
    case 'critical':
    case 'high':
      return 'bg-red-500/15 text-red-400 border-red-500/30';
    case 'medium':
      return 'bg-amber-500/15 text-amber-400 border-amber-500/30';
    case 'low':
      return 'bg-green-500/15 text-green-400 border-green-500/30';
    default:
      return 'bg-slate-500/15 text-slate-300 border-slate-500/30';
  }
}

export function SeverityBadge({ severity }: SeverityBadgeProps) {
  return (
    <span
      className={`inline-block text-xs font-semibold uppercase tracking-wide px-2.5 py-1 rounded-full border ${severityStyle(severity)}`}
    >
      {severity || 'Unknown'} severity
    </span>
  );
}
