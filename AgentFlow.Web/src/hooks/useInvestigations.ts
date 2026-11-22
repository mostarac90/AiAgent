
import {
  useMutation,
  useQuery,
} from '@tanstack/react-query';

import {
  createInvestigation,
  getInvestigation,
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
