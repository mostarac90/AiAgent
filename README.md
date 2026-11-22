# AgentFlow

AgentFlow is a small AI agent that investigates support tickets. You give it a ticket ID, and it looks up the ticket, checks who the customer is, searches the knowledge base and comes back with a summary: severity, the likely causes and what to do next.

If the agent decides a ticket needs to be escalated, it stops and asks a human for approval before it continues. I wanted the agent to be useful without being able to act on its own when the stakes are high.

The ticket data is mocked (a couple of hard-coded tickets, e.g. `1042` and `1050`). 
I will later add a database to persists the data and history.

## How it works

1. The web app sends a ticket ID to the API.
2. The API starts the investigation in the background and returns an ID right away.
3. The agent calls tools on an MCP server to fetch the ticket, the customer and knowledge base articles.
4. A workflow either writes a report directly, or pauses and waits for a human to approve or reject the escalation.
5. The web app polls the API and shows the status, the approval prompt and the final result.

## Project structure

| Project | What it does |
|---|---|
| `AgentFlow.Agent` | The agent and the workflow. |
| `AgentFlow.McpServer` | MCP server with the ticket tools (`GetTicket`, `GetCustomer`, `SearchKnowledgeBase`). |
| `AgentFlow.Api` | ASP.NET Core API that runs investigations and handles approvals. |
| `AgentFlow.Web` | React frontend. |

## Techniques used

**Backend**
- **.NET 10 and ASP.NET Core Minimal APIs** for the REST API, with OpenAPI and Swagger UI.
- **Model Context Protocol (MCP).** The tools live in a separate MCP server that the agent starts and talks to over stdio, so the agent and its tools are decoupled.
- **OpenAI tool calling** (`gpt-5-mini`). The agent runs a loop where the model chooses which MCP tool to call, with an allow-list and a cap on the number of iterations.
- **Microsoft Agent Framework Workflows.** The investigation is built as a graph of executors with conditional edges, plus a request port for the human-in-the-loop step.
- **Human-in-the-loop with `TaskCompletionSource`.** The workflow waits on a task that only completes when someone approves or rejects the escalation through the API.
- **Background processing** with in-memory sessions, so the API responds immediately while the agent keeps working.

**Frontend**
- **React 19 + TypeScript**, built with **Vite**.
- **TanStack Query** for fetching data and polling status.
- **Axios** for HTTP and **Tailwind CSS** for styling.

## Deployment

The app is deployed to Azure. The backend is deployed to Azure App service while the frontend is deployed to Azure Static Web Apps.

<b></bPreview>Preview link:</b>

https://ambitious-grass-05544d80f.5.azurestaticapps.net/



---

Developed by [Jasmin Krhan](https://www.linkedin.com/in/jasmin-krhan-031a84b1/)
