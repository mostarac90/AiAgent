import { useState } from 'react';

const storageKey = 'agentflow.howItWorks.open';

function readInitialOpen(): boolean {
  try {
    return localStorage.getItem(storageKey) !== 'false';
  } catch {
    return true;
  }
}

const steps = [
  {
    title: 'Enter a ticket ID',
    text: 'Type the ID of a support ticket and click "Start investigation". The demo has two tickets: 1042 (an API outage that needs escalation) and 1050 (a sign-up problem the agent can handle on its own).',
  },
  {
    title: 'Watch the agent work',
    text: 'The agent reads the ticket, looks up the customer and searches the knowledge base. Each step appears under "Progress" as it happens. This usually takes 10–30 seconds.',
  },
  {
    title: 'Approve or reject if asked',
    text: 'If the agent thinks the ticket should be escalated, it pauses and shows "Your approval is needed", with the reason and what approving would mean. Read it, then click "Approve escalation" or "Reject". The agent never changes a ticket by itself.',
  },
  {
    title: 'Read the result',
    text: 'When it finishes you get the findings: severity, possible causes and recommended actions, plus the outcome. Open "Full report" for the complete text.',
  },
];

interface HowItWorksProps {
  /** "panel" is collapsible (narrow screens); "sidebar" is always expanded and compact. */
  variant?: 'panel' | 'sidebar';
}

function StepList({ compact }: { compact: boolean }) {
  return (
    <div className={compact ? 'space-y-4' : 'mt-4 space-y-4'}>
      <ol className="space-y-3">
        {steps.map((step, i) => (
          <li key={step.title} className="flex gap-3">
            <span
              className={`shrink-0 rounded-full bg-blue-600/20 text-blue-400 font-semibold flex items-center justify-center ${
                compact ? 'w-6 h-6 text-xs' : 'w-7 h-7 text-sm'
              }`}
            >
              {i + 1}
            </span>
            <div>
              <p className={compact ? 'text-sm font-medium' : 'font-medium'}>
                {step.title}
              </p>
              <p className="text-sm text-slate-400 mt-0.5">{step.text}</p>
            </div>
          </li>
        ))}
      </ol>

      <p className="text-sm text-slate-400 border-t border-slate-800 pt-4">
        You can start several investigations at once. They appear under
        "Active investigations". Click one to see its details. Anything
        marked{' '}
        <span className="text-amber-400 font-medium">Needs approval</span>{' '}
        is waiting for you. Finished investigations move to "Recent".
      </p>
    </div>
  );
}

export function HowItWorks({ variant = 'panel' }: HowItWorksProps) {
  const [open, setOpen] = useState(readInitialOpen);

  if (variant === 'sidebar') {
    return (
      <section className="bg-slate-900 rounded-xl p-5">
        <h2 className="text-lg font-semibold mb-4">How it works</h2>
        <StepList compact />
      </section>
    );
  }

  function toggle() {
    const next = !open;
    setOpen(next);
    try {
      localStorage.setItem(storageKey, String(next));
    } catch {
      // Storage unavailable; the panel just won't remember its state.
    }
  }

  return (
    <section className="bg-slate-900 rounded-xl p-6">
      <button
        onClick={toggle}
        aria-expanded={open}
        className="w-full flex items-center justify-between text-left"
      >
        <h2 className="text-xl font-semibold">How it works</h2>
        <span className="text-sm text-slate-400">
          {open ? 'Hide' : 'Show'}
        </span>
      </button>

      {open && <StepList compact={false} />}
    </section>
  );
}
