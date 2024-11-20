import { createSlice } from "@reduxjs/toolkit";
import { useCallback } from "react";
import { useSelector, useDispatch } from "react-redux";

export const lpBreadcrumbsSlice = createSlice({
  name: "breadcrumbs",
  initialState: {
    data: {},
    crumbs: []
  },
  reducers: {
    setData: (state, action) => {
      state.data[action.payload.id] = action.payload.value;
    },
    deleteData: (state, action) => {
      delete state.data[action.payload];
    },
    clearData: (state) => {
      state.data = {};
    },
    setCrumbs: (state, action) => {
      state.crumbs = action.payload || [];
    },
  }
});
export const lpBreadcrumbsReducer = lpBreadcrumbsSlice.reducer;

export function useLpBreadcrumbs() {
  const dispatch = useDispatch();
  const actions = lpBreadcrumbsSlice.actions;

  const useData = () => useSelector((s) => s.breadcrumbs.data);
  const setData = useCallback(
    (id, value) => dispatch(actions.setData({ id, value })),
    [actions, dispatch]
  );
  const deleteData = useCallback(
    (id) => dispatch(actions.deleteData(id)),
    [actions, dispatch]
  );
  const clearData = useCallback(
    () => dispatch(actions.clearData()),
    [actions, dispatch]
  );


  const useCrumbs = () => useSelector((s) => s.breadcrumbs.crumbs);
  const setCrumbs = useCallback(
    (crumbs) => dispatch(actions.setCrumbs(crumbs)),
    [actions, dispatch]
  );

  const buildCrumb = (id, to, title) => ({id, to, title});

  return {
    useData,
    setData,
    deleteData,
    clearData,

    useCrumbs,
    setCrumbs,
    buildCrumb
  };
}
