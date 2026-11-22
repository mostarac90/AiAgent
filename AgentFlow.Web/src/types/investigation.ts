export type InvestigationStatus =
  | 'Running'
  | 'WaitingForApproval'
  | 'Completed'
  | 'Failed';

export type ApprovalDecision = 'Approved' | 'Rejected';

export interface CreateInvestigationRequest {
  ticketId: number;
}

export interface CreateInvestigationResponse {
  id: string;
  status: 'Running';
}

export interface TicketInvestigationResult {
  ticketId: number;
  customer: string;
  severity: string;
  issue: string;
  possibleCauses: string[];
  recommendedActions: string[];
  requiresEscalation: boolean;
  escalationReason: string;
  proposedAction: string;
}

export interface WorkflowRunResult {
  success: boolean;
  output: string | null;
}

export interface InvestigationStep {
  message: string;
  timestamp: string;
}

export interface Investigation {
  id: string;
  ticketId: number;
  status: InvestigationStatus;
  createdAt: string;
  completedAt: string | null;
  steps: InvestigationStep[];
  findings: TicketInvestigationResult | null;
  decision: ApprovalDecision | null;
  result: WorkflowRunResult | null;
  error: string | null;
}

export interface ApprovalRequest {
  approved: boolean;
}

export interface ApprovalResponse {
  message: string;
  approved: boolean;
}
