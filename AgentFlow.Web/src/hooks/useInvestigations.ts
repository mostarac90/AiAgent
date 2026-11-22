
import {
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';

import {
  createInvestigation,
  getInvestigation,
  submitApproval,
} from '../api/investigations';

export function useCreateInvestigation() {
  return useMutation({
    mutationFn: createInvestigation,
  });
}

export function useInvestigation(id: string | null) {
  return useQuery({
    queryKey: ['investigation', id],

    queryFn: () => getInvestigation(id!),

    enabled: Boolean(id),

    refetchInterval: (query) => {
      const status = query.state.data?.status;

      if (
        status === 'Completed' ||
        status === 'Failed' ||
        status === 'WaitingForApproval'
      ) {
        return false;
      }

      return 2000;
    },
  });
}

export function useSubmitApproval(investigationId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (approved: boolean) =>
      submitApproval(investigationId, { approved }),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['investigation', investigationId],
      });
    },
  });
}