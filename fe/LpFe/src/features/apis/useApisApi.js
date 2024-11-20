import { useLpApiRequest } from "../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";
import axios from "axios";

export const apisApiSlice = createSlice({
  name: "apis/api",
  initialState: {
    apis: {
      pending: false,
      value: null
    },
    lpApis: {
      pending: false,
      value: null
    },
    install: {
      pending: false,
      validation: null
    },
    uninstall: {
      pending: false,
    }
  },
  reducers: {
    startGetApis: (state) => {
      state.apis.pending = true;
      state.apis.value = null;
    },
    finishGetApis: (state, action) => {
      state.apis.pending = false;
      state.apis.value = action.payload;
    },
    startGetLpApis: (state) => {
      state.lpApis.pending = true;
      state.lpApis.value = null;
    },
    finishGetLpApis: (state, action) => {
      state.lpApis.pending = false;
      state.lpApis.value = action.payload;
    },
    startInstall: (state) => {
      state.install.pending = true;
      state.install.validation = null;
    },
    finishInstall: (state, action) => {
      state.install.pending = false;
      state.install.validation = action.payload;
    },
    startUninstall: (state) => {
      state.uninstall.pending = true;
    },
    finishUninstall: (state) => {
      state.uninstall.pending = false;
    }
  }
});
export const apisApiReducer = apisApiSlice.reducer;

export function useApisApi() {
  const baseEndpoint = `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/api`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = apisApiSlice.actions;
  const { t } = useTranslation(["lp-ui-shell"]);

  const getApis = useLpApiRequest({
    queryProvider: (ahp) => axios.get(baseEndpoint, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetApis()),
    onSuccess: (r) => dispatch(actions.finishGetApis(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetApis(null));
      enqueueSnackbar(t("lp-ui-shell:s.apis.get.error"), { variant: "error" });
    }
  });

  const getLpApis = useLpApiRequest({
    queryProvider: (ahp) => axios.get(`${baseEndpoint}/lp`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetLpApis()),
    onSuccess: (r) => dispatch(actions.finishGetLpApis(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetLpApis(null));
      enqueueSnackbar(t("lp-ui-shell:s.apis.get.error"), { variant: "error" });
    }
  });

  const installApi = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      baseEndpoint,
      ctx.callbackUrl,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startInstall()),
    onSuccess: () => {
      dispatch(actions.finishInstall(null));
      enqueueSnackbar(t("lp-ui-shell:s.apis.install.success"));
      getApis();
      getLpApis();
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishInstall(r.payload.validation));
      } else {
        dispatch(actions.finishInstall(null));
        enqueueSnackbar(t("lp-ui-shell:s.apis.install.error"), { variant: "error" });
      }
    }
  });

  const installLpApi = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/lp`,
      ctx.apiName,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startInstall()),
    onSuccess: () => {
      dispatch(actions.finishInstall(null));
      enqueueSnackbar(t("lp-ui-shell:s.apis.install.success"));
      getApis();
      getLpApis();
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishInstall(r.payload.validation));
      } else {
        dispatch(actions.finishInstall(null));
        enqueueSnackbar(t("lp-ui-shell:s.apis.install.error"), { variant: "error" });
      }
    }
  });  

  const uninstallApi = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.delete(
      baseEndpoint,
      { headers: { ...ahp() }, data: ctx.apiId }),
    onPending: () => dispatch(actions.startUninstall()),
    onSuccess: () => {
      dispatch(actions.finishUninstall());
      enqueueSnackbar(t("lp-ui-shell:s.apis.uninstall.success"));
      getApis();
      getLpApis();
    },
    onError: () => {
      dispatch(actions.finishUninstall());
      enqueueSnackbar(t("lp-ui-shell:s.apis.uninstall.error"), { variant: "error" });
    }
  });  

  return {
    state: {
      apis: useSelector((s) => s.apis.api.apis),
      lpApis: useSelector((s) => s.apis.api.lpApis),
      install: useSelector((s) => s.apis.api.install),
      uninstall: useSelector((s) => s.apis.api.uninstall),
    },
    actions: {
      getApis,
      getLpApis,
      installApi,
      installLpApi,
      uninstallApi
    }
  };
}