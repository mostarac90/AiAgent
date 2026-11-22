
import {
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';

import {
  createInvestigation,
  getActiveInvestigations,
  getInvestigation,
  submitApproval,
} from '../api/investigations';

const activeInvestigationsKey = ['investigations', 'active'];

export function useCreateInvestigation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createInvestigation,

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: activeInvestigationsKey,
      });
    },
  });
}

export function useActiveInvestigations() {
  return useQuery({
    queryKey: activeInvestigationsKey,
    queryFn: getActiveInvestigations,
    refetchInterval: 3000,
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
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ['investigation', investigationId],
        }),
        queryClient.invalidateQueries({
          queryKey: activeInvestigationsKey,
        }),
      ]);
    },
  });
}