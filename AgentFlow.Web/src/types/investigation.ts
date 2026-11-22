export type InvestigationStatus =
  | 'Running'
  | 'WaitingForApproval'
  | 'Completed'
  | 'Failed';


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
}

export interface WorkflowRunResult {
  success: boolean;
  output: string | null;
}

export interface Investigation {
  id: string;
  status: InvestigationStatus;
  approvalRequest: TicketInvestigationResult | null;
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
