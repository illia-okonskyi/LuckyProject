import { useLpApiRequest } from "../../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";

import { useAppAuthStore } from "../../useAppAuthStore";

import axios from "axios";
import { useCallback } from "react";

export const adminPanelClientsApiSlice = createSlice({
  name: "apis/adminPanel/clients",
  initialState: {
    list: null,
    single: null,
    secret: null,
    createWeb: {
      pending: false,
      validation: null
    },
    createMachine: {
      pending: false,
      validation: null
    },
    resetMachineSecret: {
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
    resetSingle: (state) => {
      state.single = null;
      state.createWeb.validation = null;
      state.createMachine.validation = null;
      state.resetMachineSecret.validation = null;
    },
    startGetSecret: (state) => {
      state.secret = null;
    },
    finishGetSecret: (state, action) => {
      state.secret = action.payload;
    },
    startCreateWeb: (state) => {
      state.createWeb.pending = true;
      state.createWeb.value = null;
    },
    finishCreateWebSuccess: (state, action) => {
      state.createWeb.pending = false;
      state.single = action.payload;
      state.createWeb.validation = null;
    },
    finishCreateWebValidation: (state, action) => {
      state.createWeb.pending = false;
      state.createWeb.validation = action.payload;
    },
    startCreateMachine: (state) => {
      state.createMachine.pending = true;
      state.createMachine.value = null;
    },
    finishCreateMachineSuccess: (state, action) => {
      state.createMachine.pending = false;
      state.single = action.payload;
      state.createMachine.validation = null;
    },
    finishCreateMachineValidation: (state, action) => {
      state.createMachine.pending = false;
      state.createMachine.validation = action.payload;
    },
    startResetMachineSecret: (state) => {
      state.resetMachineSecret.pending = true;
      state.resetMachineSecret.validation = null;
    },
    finishResetMachineSecret: (state, action) => {
      state.resetMachineSecret.pending = false;
      state.resetMachineSecret.validation = action.payload;
    },
    startDelete: (state) => {
      state.delete.pending = true;
    },
    finishDelete: (state) => {
      state.delete.pending = false;
    },
  }
});
export const adminPanelClientsApiReducer = adminPanelClientsApiSlice.reducer;

export function useAdminPanelClientsApi() {
  const { getApiEndpoint } = useAppAuthStore();
  const baseEndpoint = `${getApiEndpoint("Admin-Panel")}/clients`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = adminPanelClientsApiSlice.actions;
  const { t } = useTranslation(["lp-ui-admin-panel"]);

  const getClientsList = useLpApiRequest({
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
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.getList.failed"), { variant: "error" });
    }
  });

  const getClient = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.get(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGet()),
    onSuccess: (r) => dispatch(actions.finishGet(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGet(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.get.failed"), { variant: "error" });
    }
  });

  const getMachineClientSecret = useLpApiRequest({
    queryProvider: (ahp) => axios.get(`${baseEndpoint}/secret`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetSecret()),
    onSuccess: (r) => dispatch(actions.finishGetSecret(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetSecret(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.getSecret.failed"), { variant: "error" });
    }
  });  

  const resetClient = useCallback(() => dispatch(actions.resetSingle()), [actions, dispatch]);

  const createWebClient = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/web`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startCreateWeb()),
    onSuccess: (r) => {
      dispatch(actions.finishCreateWebSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.createWeb.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishCreateWebValidation(r.payload.validation));
      } else {
        dispatch(actions.finishCreateWebValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.createWeb.failed"), { variant: "error" });
      }
    }
  });

  const createMachineClient = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/machine`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startCreateMachine()),
    onSuccess: (r) => {
      dispatch(actions.finishCreateMachineSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.createMachine.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishCreateMachineValidation(r.payload.validation));
      } else {
        dispatch(actions.finishCreateMachineValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.createMachine.failed"), { variant: "error" });
      }
    }
  });

  const resetMachineClientSecret = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(
      `${baseEndpoint}/machine-secret`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startResetMachineSecret()),
    onSuccess: () => {
      dispatch(actions.finishResetMachineSecret(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.resetMachineSecret.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishResetMachineSecret(r.payload.validation));
      } else {
        dispatch(actions.finishResetMachineSecret(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.resetMachineSecret.failed"), { variant: "error" });
      }
    }
  });

  const deleteClient = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.delete(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startDelete()),
    onSuccess: (r) => {
      dispatch(actions.finishDelete());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.delete.success"));
      resetClient();
      if (r.ctx.fop) {
        getClientsList({ fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishDelete(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.clients.delete.failed"), { variant: "error" });
    }
  });

  return {
    state: {
      list: useSelector((s) => s.apis.adminPanel.clients.list),
      single: useSelector((s) => s.apis.adminPanel.clients.single),
      secret: useSelector((s) => s.apis.adminPanel.clients.secret),
      createWeb: useSelector((s) => s.apis.adminPanel.clients.createWeb),
      createMachine: useSelector((s) => s.apis.adminPanel.clients.createMachine),
      resetMachineSecret: useSelector((s) => s.apis.adminPanel.clients.resetMachineSecret),
      deleteState: useSelector((s) => s.apis.adminPanel.clients.delete),
    },
    actions: {
      getClientsList,
      getClient,
      resetClient,
      getMachineClientSecret,
      createWebClient,
      createMachineClient,
      resetMachineClientSecret,
      deleteClient
    }
  };
}