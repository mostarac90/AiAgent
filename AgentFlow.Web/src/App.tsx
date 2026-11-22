
import { useState } from 'react';
import { ApprovalPanel } from './components/ApprovalPanel';

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
    <main className="min-h-screen bg-slate-950 text-white p-8">
      <div className="max-w-3xl mx-auto space-y-6">
        <header>
          <h1 className="text-4xl font-bold">
            AgentFlow
          </h1>
          <p className="text-slate-400 mt-2">
            AI-powered ticket investigation
          </p>
        </header>

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

        {investigationId && (
          <section className="bg-slate-900 rounded-xl p-6 space-y-4">
            <h2 className="text-xl font-semibold">
              Investigation details
            </h2>

            <p className="text-slate-400 text-sm break-all">
              ID: {investigationId}
            </p>

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
                    {investigation.status}
                  </span>
                </p>

                {investigation.status === 'Running' && (
                  <p className="text-yellow-400">
                    Agent is investigating ticket...
                  </p>
                )}

                {investigation.status ===
                  'WaitingForApproval' && (
                  <p className="text-amber-400">
                    Human approval is required.
                  </p>
                )}

                {investigation.result && (
                  <div className="bg-slate-800 rounded-lg p-4">
                    <h3 className="font-semibold mb-2">
                      Agent findings
                    </h3>
                    <pre className="whitespace-pre-wrap break-words">
                      {investigation.result.output}
                    </pre>
                  </div>
                )}

                {investigation.error && (
                  <p className="text-red-400">
                    {investigation.error}
                  </p>
                )}
              </>
            )}
            {investigation?.status === 'WaitingForApproval' && (
              <ApprovalPanel investigationId={investigationId} />
            )}
          </section>
        )}
      </div>
    </main>
  );
}
