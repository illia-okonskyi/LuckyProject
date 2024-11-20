import { useLpApiRequest } from "../../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";

import { useAppAuthStore } from "../../useAppAuthStore";

import axios from "axios";

export const adminPanelPermissionsApiSlice = createSlice({
  name: "apis/adminPanel/permissions",
  initialState: {
    list: null,
    single: null,
    update: {
      pending: false,
      validation: null
    },
    delete: {
      pending: false
    }
  },
  reducers: {
    startGetList: (state) => {
      state.list = null;
    },
    finishGetList: (state, action) => {
      state.list = action.payload;
    },
    startGet: (state) => {
      state.single = null;
    },
    finishGet: (state, action) => {
      state.single = action.payload;
    },
    startUpdate: (state) => {
      state.update.pending = true;
      state.update.value = null;
    },
    finishUpdateSuccess: (state, action) => {
      state.update.pending = false;
      state.single = action.payload;
      state.update.validation = null;
    },
    finishUpdateValidation: (state, action) => {
      state.update.pending = false;
      state.update.validation = action.payload;
    },
    startDelete: (state) => {
      state.delete.pending = true;
    },
    finishDelete: (state) => {
      state.delete.pending = false;
    }
  }
});
export const adminPanelPermissionsApiReducer = adminPanelPermissionsApiSlice.reducer;

export function useAdminPanelPermissionsApi() {
  const { getApiEndpoint } = useAppAuthStore();
  const baseEndpoint = `${getApiEndpoint("Admin-Panel")}/permissions`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = adminPanelPermissionsApiSlice.actions;
  const { t } = useTranslation(["lp-ui-admin-panel"]);

  const getPermissionsList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/list`,
      {
        ...ctx.fop,
        filters: {
          ...ctx.fop.filters,
          type: ctx.fop.filters.type !== "Any" ? ctx.fop.filters.type : null
        }
      },
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetList()),
    onSuccess: (r) => dispatch(actions.finishGetList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.getList.failed"), { variant: "error" });
    }
  });

  const getPermission = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.get(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGet()),
    onSuccess: (r) => dispatch(actions.finishGet(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGet(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.get.failed"), { variant: "error" });
    }
  });  

  const updatePermission = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(baseEndpoint, ctx.request, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startUpdate()),
    onSuccess: (r) => {
      dispatch(actions.finishUpdateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.update.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishUpdateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishUpdateValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.update.failed"), { variant: "error" });
      }
    }
  });

  const deletePermission = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.delete(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startDelete()),
    onSuccess: (r) => {
      dispatch(actions.finishDelete());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.delete.success"));
      if (r.ctx.fop) {
        getPermissionsList({ fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishDelete(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.permissions.delete.failed"), { variant: "error" });
    }
  });

  return {
    state: {
      list: useSelector((s) => s.apis.adminPanel.permissions.list),
      single: useSelector((s) => s.apis.adminPanel.permissions.single),
      update: useSelector((s) => s.apis.adminPanel.permissions.update),
      deleteState: useSelector((s) => s.apis.adminPanel.permissions.delete),
    },
    actions: {
      getPermissionsList,
      getPermission,
      updatePermission,
      deletePermission
    }
  };
}