import { useLpApiRequest } from "../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";
import axios from "axios";

export const authProfileSlice = createSlice({
  name: "auth/profile",
  initialState: {
    profile: null,
    get: {
      pending: false
    },
    locales: {
      pending: false,
      value: null
    },
    update: {
      pending: false,
      validation: null
    },
    updatePassword: {
      pending: false,
      validation: null
    }
  },
  reducers: {
    startGet: (state) => {
      state.get.pending = true;
      state.profile = null;
    },
    finishGet: (state, action) => {
      state.get.pending = false;
      state.profile = action.payload;
    },
    startGetLocales: (state) => {
      state.locales.pending = true;
      state.locales.value = null;
    },
    finishGetLocales: (state, action) => {
      state.locales.pending = false;
      state.locales.value = action.payload;
    },    
    startUpdate: (state) => {
      state.update.pending = true;
    },
    finishUpdateSuccess: (state, action) => {
      state.update.pending = false;
      state.update.validation = null;
      state.profile = action.payload;
    },
    finishUpdateValidation: (state, action) => {
      state.update.pending = false;
      state.update.validation = action.payload;
    },
    finishUpdateError: (state) => {
      state.update.pending = false;
      state.update.validation = null;
    },
    startUpdatePassword: (state) => {
      state.updatePassword.pending = true;
      state.updatePassword.validation = null;
    },
    finishUpdatePassword: (state, action) => {
      state.updatePassword.pending = false;
      state.updatePassword.validation = action.payload;
    },
  }
});
export const authProfileReducer = authProfileSlice.reducer;

export function useAuthProfile() {
  const baseEndpoint = `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/profile`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = authProfileSlice.actions;
  const { t } = useTranslation(["lp-ui-shell"]);

  const getProfile = useLpApiRequest({
    queryProvider: (ahp) => axios.get(baseEndpoint, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGet()),
    onSuccess: (r) => dispatch(actions.finishGet(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGet(null));
      enqueueSnackbar(t("lp-ui-shell:s.profile.get.error"), { variant: "error" });
    }
  });

  const getLocales = useLpApiRequest({
    queryProvider: (ahp) => axios.get(`${baseEndpoint}/locales`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetLocales()),
    onSuccess: (r) => dispatch(actions.finishGetLocales(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetLocales(null));
      enqueueSnackbar(t("lp-ui-shell:s.profile.getLocales.error"), { variant: "error" });
    }
  });

  const updateProfile = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(baseEndpoint, ctx.request, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startUpdate()),
    onSuccess: (r) => {
      dispatch(actions.finishUpdateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-shell:s.profile.update.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishUpdateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishUpdateError());
        enqueueSnackbar(t("lp-ui-shell:s.profile.update.error"), { variant: "error" });
      }
    }
  });

  const updateUserPassword = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(
      `${baseEndpoint}/password`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startUpdatePassword()),
    onSuccess: () => {
      dispatch(actions.finishUpdatePassword(null));
      enqueueSnackbar(t("lp-ui-shell:s.profile.updatePassword.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishUpdatePassword(r.payload.validation));
      } else {
        dispatch(actions.finishUpdatePassword());
        enqueueSnackbar(t("lp-ui-shell:s.profile.updatePassword.error"), { variant: "error" });
        }
    }
  });

  return {
    state: {
      profile: useSelector((s) => s.auth.profile.profile),
      locales: useSelector((s) => s.auth.profile.locales),
      get: useSelector((s) => s.auth.profile.get),
      update: useSelector((s) => s.auth.profile.update),
      updatePassword: useSelector((s) => s.auth.profile.updatePassword)
    },
    actions: {
      getProfile,
      getLocales,
      updateProfile,
      updateUserPassword
    }
  };
}