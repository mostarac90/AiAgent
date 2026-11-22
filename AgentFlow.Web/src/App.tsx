
import { useState } from 'react';
import { apiClient } from './api/client';

export default function App() {
  const [ticketId, setTicketId] = useState('');
  const [result, setResult] = useState<unknown>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function startInvestigation() {
    const id = Number(ticketId);

    if (!Number.isSafeInteger(id) || id <= 0) {
      setError('Enter a valid positive ticket ID.');
      return;
    }

    setLoading(true);
    setError('');
    setResult(null);

    try {
      const response = await apiClient.post(
        '/investigations',
        { ticketId: id }
      );

      setResult(response.data);
    } catch (err) {
      setError('Failed to start investigation.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="min-h-screen bg-slate-950 text-white p-8 flex flex-col">
      <div className="mx-auto max-w-3xl w-full flex-1">
        <h1 className="text-4xl font-bold mb-2">
          AgentFlow
        </h1>

        <p className="text-slate-400 mb-8">
          AI-powered ticket investigation platform
        </p>

        <div className="rounded-xl bg-slate-900 p-6 border border-slate-800">
          <h2 className="text-xl font-semibold mb-4">
            New investigation
          </h2>

          <input
            type="number"
            min="1"
            value={ticketId}
            onChange={(e) => setTicketId(e.target.value)}
            placeholder="Enter ticket ID (e.g. 1042)"
            className="w-full bg-slate-800 rounded-lg p-3 mb-4"
          />

          <button
            onClick={startInvestigation}
            disabled={loading}
            className="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 px-6 py-3 rounded-lg"
          >
            {loading ? 'Starting...' : 'Start investigation'}
          </button>

          {error && (
            <p className="mt-4 text-red-400">{error}</p>
          )}
        </div>

        {result !== null && (
          <pre className="mt-6 bg-slate-900 p-6 rounded-xl overflow-auto">
            {JSON.stringify(result, null, 2)}
          </pre>
        )}
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
