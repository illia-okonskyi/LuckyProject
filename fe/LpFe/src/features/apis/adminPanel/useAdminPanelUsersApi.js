import { useLpApiRequest } from "../../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";

import { useAppAuthStore } from "../../useAppAuthStore";

import axios from "axios";
import { useCallback } from "react";

export const adminPanelUsersApiSlice = createSlice({
  name: "apis/adminPanel/users",
  initialState: {
    list: null,
    single: null,
    create: {
      pending: false,
      validation: null
    },
    update: {
      pending: false,
      validation: null
    },
    resetPassword: {
      pending: false,
      validation: null
    },
    delete: {
      pending: false
    },
    rolesList: null,
    assignDeleteRole: {
      pending: false
    },
    permissionsList: null
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
      state.create.validation = null;
      state.update.validation = null;
      state.resetPassword.validation = null;
    },
    startCreate: (state) => {
      state.create.pending = true;
      state.create.value = null;
    },
    finishCreateSuccess: (state, action) => {
      state.create.pending = false;
      state.single = action.payload;
      state.create.validation = null;
    },
    finishCreateValidation: (state, action) => {
      state.create.pending = false;
      state.create.validation = action.payload;
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
    startResetPassword: (state) => {
      state.resetPassword.pending = true;
      state.resetPassword.validation = null;
    },
    finishResetPassword: (state, action) => {
      state.resetPassword.pending = false;
      state.resetPassword.validation = action.payload;
    },
    startDelete: (state) => {
      state.delete.pending = true;
    },
    finishDelete: (state) => {
      state.delete.pending = false;
    },
    startGetRolesList: (state) => {
      state.rolesList = null;
    },
    finishGetRolesList: (state, action) => {
      state.rolesList = action.payload;
    },
    startAssignDeleteRole: (state) => {
      state.assignDeleteRole.pending = true;
    },
    finishAssignDeleteRole: (state) => {
      state.assignDeleteRole.pending = false;
    },
    startGetPermissionsList: (state) => {
      state.permissionsList = null;
    },
    finishGetPermissionsList: (state, action) => {
      state.permissionsList = action.payload;
    },
  }
});
export const adminPanelUsersApiReducer = adminPanelUsersApiSlice.reducer;

export function useAdminPanelUsersApi() {
  const { getApiEndpoint } = useAppAuthStore();
  const baseEndpoint = `${getApiEndpoint("Admin-Panel")}/users`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = adminPanelUsersApiSlice.actions;
  const { t } = useTranslation(["lp-ui-admin-panel"]);

  const getUsersList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/list`,
      {
        ...ctx.fop,
        filters: {
          ...ctx.fop.filters,
          preferredLocale: ctx.fop.filters.preferredLocale !== "Any"
            ? ctx.fop.filters.preferredLocale
            : null
        }
      },
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetList()),
    onSuccess: (r) => dispatch(actions.finishGetList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.getList.failed"), { variant: "error" });
    }
  });

  const getUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.get(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGet()),
    onSuccess: (r) => dispatch(actions.finishGet(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGet(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.get.failed"), { variant: "error" });
    }
  });

  const resetUser = useCallback(() => dispatch(actions.resetSingle()), [actions, dispatch]);

  const createUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(baseEndpoint, ctx.request, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startCreate()),
    onSuccess: (r) => {
      dispatch(actions.finishCreateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.create.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishCreateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishCreateValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.create.failed"), { variant: "error" });
      }
    }
  });

  const updateUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => {
      const postfix = ctx.isWeb ? "/web" : "/machine";
      return axios.put(baseEndpoint + postfix, ctx.request, { headers: { ...ahp() } });
    },
    onPending: () => dispatch(actions.startUpdate()),
    onSuccess: (r) => {
      dispatch(actions.finishUpdateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.update.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishUpdateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishUpdateValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.update.failed"), { variant: "error" });
      }
    }
  });

  const resetWebUserPassword = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(
      `${baseEndpoint}/web-password`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startResetPassword()),
    onSuccess: () => {
      dispatch(actions.finishResetPassword(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.resetPassword.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishResetPassword(r.payload.validation));
      } else {
        dispatch(actions.finishResetPassword(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.resetPassword.failed"), { variant: "error" });
      }
    }
  });

  const deleteUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.delete(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startDelete()),
    onSuccess: (r) => {
      dispatch(actions.finishDelete());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.delete.success"));
      resetUser();
      if (r.ctx.fop) {
        getUsersList({ fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishDelete(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.delete.failed"), { variant: "error" });
    }
  });

  const getUserRolesList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/${ctx.id}/roles`,
      ctx.fop,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetRolesList()),
    onSuccess: (r) => dispatch(actions.finishGetRolesList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetRolesList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.getRoles.failed"), { variant: "error" });
    }
  });

  const assignRoleToUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/assign-role`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeleteRole()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeleteRole());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.assignRole.success"));
      if (r.ctx.fop) {
        getUserRolesList({ id: r.ctx.request.userId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeleteRole(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.assignRole.failed"), { variant: "error" });
    }
  });  

  const deleteRoleFromUser = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/delete-role`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeleteRole()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeleteRole());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.deleteRole.success"));
      if (r.ctx.fop) {
        getUserRolesList({ id: r.ctx.request.userId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeleteRole(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.deleteRole.failed"), { variant: "error" });
    }
  });      

  const getUserPermissionsList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/${ctx.id}/permissions`,
      {
        ...ctx.fop,
        filters: {
          ...ctx.fop.filters,
          type: ctx.fop.filters.type !== "Any" ? ctx.fop.filters.type : null
        }
      },
      { headers: { ...ahp() }
    }),
    onPending: () => dispatch(actions.startGetPermissionsList()),
    onSuccess: (r) => dispatch(actions.finishGetPermissionsList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetPermissionsList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.users.getPermissions.failed"), { variant: "error" });
    }
  });

  return {
    state: {
      list: useSelector((s) => s.apis.adminPanel.users.list),
      single: useSelector((s) => s.apis.adminPanel.users.single),
      create: useSelector((s) => s.apis.adminPanel.users.create),
      update: useSelector((s) => s.apis.adminPanel.users.update),
      resetPassword: useSelector((s) => s.apis.adminPanel.users.resetPassword),
      deleteState: useSelector((s) => s.apis.adminPanel.users.delete),
      rolesList: useSelector((s) => s.apis.adminPanel.users.rolesList),
      assignDeleteRole: useSelector((s) => s.apis.adminPanel.users.assignDeleteRole),
      permissionsList: useSelector((s) => s.apis.adminPanel.users.permissionsList)
    },
    actions: {
      getUsersList,
      getUser,
      resetUser,
      createUser,
      updateUser,
      resetWebUserPassword,
      deleteUser,

      getUserRolesList,
      assignRoleToUser,
      deleteRoleFromUser,
      
      getUserPermissionsList
    }
  };
}