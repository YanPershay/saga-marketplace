import { useQuery } from "@tanstack/react-query";
import { getProducts } from "../../../api/catalogApi";
import { queryKeys } from "../../../lib/queryKeys";

export function useProductsQuery() {
  return useQuery({
    queryKey: queryKeys.products.all,
    queryFn: getProducts,
  });
}
