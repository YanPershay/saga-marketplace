import { useQuery } from "@tanstack/react-query";
import { getOrder } from "../../../api/ordersApi";
import { isTerminalOrderStatus } from "../../../lib/orderStatus";
import { queryKeys } from "../../../lib/queryKeys";

export function useOrderQuery(orderId?: string) {
  return useQuery({
    queryKey: queryKeys.orders.detail(orderId ?? ""),
    queryFn: () => getOrder(orderId!),
    enabled: Boolean(orderId),
    refetchInterval: (query) =>
      isTerminalOrderStatus(query.state.data?.status) ? false : 2000,
  });
}
