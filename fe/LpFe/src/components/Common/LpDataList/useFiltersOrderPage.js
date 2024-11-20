import { useState, useCallback } from "react";

export function useFiltersOrderPage({ defaultFilters = {}, defaultOrder = null } = {}) {
  const [filters, setFilters] = useState(defaultFilters);
  const [order, setOrder] = useState(defaultOrder);
  const [page, setPage] = useState(1);

  const setFilter = useCallback((key, value) => {
    setFilters({ ...filters, [key]: value });
  }, [filters]);

  return {
    filters, setFilters, setFilter,
    order, setOrder,
    page, setPage
  };
}