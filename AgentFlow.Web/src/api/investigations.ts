
import { apiClient } from './client';

import type {
  Investigation,
  CreateInvestigationRequest,
  CreateInvestigationResponse,
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

export async function getInvestigation(
  id: string
): Promise<Investigation> {
  const response = await apiClient.get<Investigation>(
    `/investigations/${encodeURIComponent(id)}`
  );

  return response.data;
}
