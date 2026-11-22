
import { apiClient } from './client';

import type {
  Investigation,
  CreateInvestigationRequest,
  CreateInvestigationResponse,
  ApprovalRequest,
} from '../types/investigation';

export async function createInvestigation(
  request: CreateInvestigationRequest
): Promise<CreateInvestigationResponse> {
  const response =
    await apiClient.post<CreateInvestigationResponse>(
      '/investigations',
      request
    );

  return response.data;
}

export async function getInvestigations(): Promise<
  Investigation[]
> {
  const response = await apiClient.get<Investigation[]>(
    '/investigations'
  );

  return response.data;
}

export async function getInvestigation(
  id: string
): Promise<Investigation> {
  const response = await apiClient.get<Investigation>(
    `/investigations/${encodeURIComponent(id)}`
  );

  return response.data;
}


export async function submitApproval(
  investigationId: string,
  request: ApprovalRequest
): Promise<void> {
  await apiClient.post(
    `/investigations/${encodeURIComponent(investigationId)}/approval`,
    request
  );
}
