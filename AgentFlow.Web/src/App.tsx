
import { useState } from 'react';
import { ApprovalPanel } from './components/ApprovalPanel';
import { FindingsCard } from './components/FindingsCard';
import { HowItWorks } from './components/HowItWorks';
import { InvestigationList } from './components/InvestigationList';
import { ProgressTimeline } from './components/ProgressTimeline';
import type { Investigation } from './types/investigation';

const statusLabels: Record<Investigation['status'], string> = {
  Running: 'Running',
  WaitingForApproval: 'Waiting for your approval',
  Completed: 'Completed',
  Failed: 'Failed',
};

function outcomeMessage(investigation: Investigation): {
  text: string;
  style: string;
} | null {
  if (investigation.status !== 'Completed') return null;

  if (investigation.decision === 'Approved') {
    return {
      text: 'You approved the escalation. No ticket has been modified by the agent.',
      style: 'border-blue-500/40 bg-blue-500/10 text-blue-300',
    };
  }
  if (investigation.decision === 'Rejected') {
    return {
      text: 'You rejected the escalation. No further action will be taken.',
      style: 'border-slate-500/40 bg-slate-500/10 text-slate-300',
    };
  }
  return {
    text: 'No escalation was needed. The agent completed the report on its own.',
    style: 'border-green-500/40 bg-green-500/10 text-green-300',
  };
}

import {
  useCreateInvestigation,
  useInvestigation,
} from './hooks/useInvestigations';

export default function App() {
  const [ticketId, setTicketId] = useState('');
  const [investigationId, setInvestigationId] =
    useState<string | null>(null);

  const createMutation = useCreateInvestigation();

  const investigationQuery =
    useInvestigation(investigationId);

  async function handleStartInvestigation() {
    const id = Number(ticketId);

    if (!Number.isSafeInteger(id) || id <= 0) {
      return;
    }

    setInvestigationId(null);

    try {
      const response = await createMutation.mutateAsync({
        ticketId: id,
      });

      setInvestigationId(response.id);
    } catch (error) {
      console.error(error);
    }
  }

  const investigation = investigationQuery.data;

  return (
    <main className="min-h-screen bg-slate-950 text-white p-8 flex flex-col">
      {/* Wide screens: tutorial sits in a right-hand sidebar so the main column stays centred. */}
      <div className="w-full flex-1 xl:grid xl:grid-cols-[1fr_minmax(0,42rem)_1fr] xl:gap-8">
      <aside className="hidden xl:block xl:col-start-3 xl:row-start-1">
        <div className="sticky top-8 max-w-xs">
          <HowItWorks variant="sidebar" />
        </div>
      </aside>

      <div className="max-w-3xl w-full mx-auto space-y-6 xl:col-start-2 xl:row-start-1">
        <header>
          <h1 className="text-4xl font-bold">
            AgentFlow
          </h1>
          <p className="text-slate-400 mt-2">
            AI-powered ticket investigation
          </p>
        </header>

        <div className="xl:hidden">
          <HowItWorks />
        </div>

        <section className="bg-slate-900 rounded-xl p-6">
          <h2 className="text-xl font-semibold mb-4">
            Start investigation
          </h2>

          <input
            type="number"
            min="1"
            value={ticketId}
            onChange={(e) => setTicketId(e.target.value)}
            placeholder="Ticket ID"
            className="bg-slate-800 rounded-lg p-3 w-full mb-4"
          />

          <button
            onClick={handleStartInvestigation}
            disabled={
              createMutation.isPending ||
              !Number.isSafeInteger(Number(ticketId)) ||
              Number(ticketId) <= 0
            }
            className="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 px-6 py-3 rounded-lg"
          >
            {createMutation.isPending
              ? 'Starting...'
              : 'Start investigation'}
          </button>

          {createMutation.isError && (
            <p className="text-red-400 mt-4">
              Failed to create investigation.
            </p>
          )}
        </section>

        <InvestigationList
          selectedId={investigationId}
          onSelect={setInvestigationId}
        />

        {investigationId && (
          <section className="bg-slate-900 rounded-xl p-6 space-y-4">
            <div>
              <h2 className="text-xl font-semibold">
                {investigation
                  ? `Ticket #${investigation.ticketId}`
                  : 'Investigation details'}
              </h2>
              <p className="text-slate-500 text-xs break-all mt-1">
                Investigation {investigationId}
              </p>
            </div>

            {investigationQuery.isPending && (
              <p>Loading investigation...</p>
            )}

            {investigationQuery.isError && (
              <p className="text-red-400">
                Failed to retrieve investigation.
              </p>
            )}

            {investigation && (
              <>
                <p>
                  Status:{' '}
                  <span className="font-bold text-blue-400">
                    {statusLabels[investigation.status]}
                  </span>
                </p>

                <ProgressTimeline
                  steps={investigation.steps}
                  status={investigation.status}
                />

                {investigation.status === 'WaitingForApproval' ? (
                  <ApprovalPanel
                    key={investigation.id}
                    investigationId={investigation.id}
                    findings={investigation.findings}
                  />
                ) : (
                  investigation.findings && (
                    <FindingsCard findings={investigation.findings} />
                  )
                )}

                {(() => {
                  const outcome = outcomeMessage(investigation);
                  return (
                    outcome && (
                      <div className={`border rounded-lg p-4 ${outcome.style}`}>
                        {outcome.text}
                        {investigation.decision &&
                          investigation.findings?.escalationReason && (
                            <p className="text-sm text-slate-400 mt-2">
                              Reason given by the agent:{' '}
                              {investigation.findings.escalationReason}
                            </p>
                          )}
                      </div>
                    )
                  );
                })()}

                {investigation.result?.output && (
                  <details className="bg-slate-800 rounded-lg p-4">
                    <summary className="cursor-pointer font-semibold">
                      Full report
                    </summary>
                    <pre className="whitespace-pre-wrap wrap-break-word mt-3 text-sm">
                      {investigation.result.output}
                    </pre>
                  </details>
                )}

                {investigation.error && (
                  <p className="text-red-400">
                    {investigation.error}
                  </p>
                )}
              </>
            )}
          </section>
        )}
      </div>
      </div>

      <footer className="mt-12 pt-6 border-t border-slate-800 text-center text-sm text-slate-500">
        Developed by{' '}
        <a
          href="https://www.linkedin.com/in/jasmin-krhan-031a84b1/"
          target="_blank"
          rel="noopener noreferrer"
          className="text-slate-300 hover:text-blue-400 underline-offset-4 hover:underline"
        >
          Jasmin Krhan
        </a>
      </footer>
    </main>
  );
}
