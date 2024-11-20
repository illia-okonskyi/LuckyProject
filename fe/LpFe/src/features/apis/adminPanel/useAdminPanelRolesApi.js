import { useLpApiRequest } from "../../useLpApiRequest";
import { createSlice } from "@reduxjs/toolkit";
import { useSelector, useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { useTranslation } from "react-i18next";

import { useAppAuthStore } from "../../useAppAuthStore";

import axios from "axios";
import { useCallback } from "react";

export const adminPanelRolesApiSlice = createSlice({
  name: "apis/adminPanel/roles",
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
    delete: {
      pending: false
    },
    permissionsList: null,
    assignDeletePermission: {
      pending: false
    },
    usersList: null,
    assignDeleteUser: {
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
      state.create.validation = null;
      state.update.validation = null;
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
    startDelete: (state) => {
      state.delete.pending = true;
    },
    finishDelete: (state) => {
      state.delete.pending = false;
    },
    startGetPermissionsList: (state) => {
      state.permissionsList = null;
    },
    finishGetPermissionsList: (state, action) => {
      state.permissionsList = action.payload;
    },
    startAssignDeletePermission: (state) => {
      state.assignDeletePermission.pending = true;
    },
    finishAssignDeletePermission: (state) => {
      state.assignDeletePermission.pending = false;
    },
    startGetUsersList: (state) => {
      state.usersList = null;
    },
    finishGetUsersList: (state, action) => {
      state.usersList = action.payload;
    },
    startAssignDeleteUser: (state) => {
      state.assignDeleteUser.pending = true;
    },
    finishAssignDeleteUser: (state) => {
      state.assignDeleteUser.pending = false;
    }
  }
});
export const adminPanelRolesApiReducer = adminPanelRolesApiSlice.reducer;

export function useAdminPanelRolesApi() {
  const { getApiEndpoint } = useAppAuthStore();
  const baseEndpoint = `${getApiEndpoint("Admin-Panel")}/roles`;
  const { enqueueSnackbar } = useSnackbar();
  const dispatch = useDispatch();
  const actions = adminPanelRolesApiSlice.actions;
  const { t } = useTranslation(["lp-ui-admin-panel"]);

  const getRolesList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/list`,
      ctx.fop,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetList()),
    onSuccess: (r) => dispatch(actions.finishGetList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.getList.failed"), { variant: "error" });
    }
  });

  const getRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.get(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGet()),
    onSuccess: (r) => dispatch(actions.finishGet(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGet(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.get.failed"), { variant: "error" });
    }
  });

  const resetRole = useCallback(() => dispatch(actions.resetSingle()), [actions, dispatch]);

  const createRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(baseEndpoint, ctx.request, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startCreate()),
    onSuccess: (r) => {
      dispatch(actions.finishCreateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.create.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishCreateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishCreateValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.create.failed"), { variant: "error" });
      }
    }
  });

  const updateRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.put(baseEndpoint, ctx.request, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startUpdate()),
    onSuccess: (r) => {
      dispatch(actions.finishUpdateSuccess(r.payload.payload));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.update.success"));
    },
    onError: (r) => {
      if (r.payload.errorType === "client/validation") {
        dispatch(actions.finishUpdateValidation(r.payload.validation));
      } else {
        dispatch(actions.finishUpdateValidation(null));
        enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.update.failed"), { variant: "error" });
      }
    }
  });

  const deleteRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.delete(`${baseEndpoint}/${ctx.id}`, { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startDelete()),
    onSuccess: (r) => {
      dispatch(actions.finishDelete());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.delete.success"));
      resetRole();
      if (r.ctx.fop) {
        getRolesList({ fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishDelete(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.delete.failed"), { variant: "error" });
    }
  });

  const getRolePermissionsList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/${ctx.id}/permissions`,
      {
        ...ctx.fop,
        filters: {
          ...ctx.fop.filters,
          type: ctx.fop.filters.type !== "Any" ? ctx.fop.filters.type : null
        }
      },
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startGetPermissionsList()),
    onSuccess: (r) => dispatch(actions.finishGetPermissionsList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetPermissionsList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.getPermissions.failed"), { variant: "error" });
    }
  });

  const assignPermissionToRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/assign-permission`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeletePermission()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeletePermission());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.assignPermission.success"));
      if (r.ctx.fop) {
        getRolePermissionsList({ id: r.ctx.request.roleId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeletePermission(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.assignPermission.failed"), { variant: "error" });
    }
  });  

  const deletePermissionFromRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/delete-permission`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeletePermission()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeletePermission());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.deletePermission.success"));
      if (r.ctx.fop) {
        getRolePermissionsList({ id: r.ctx.request.roleId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeletePermission(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.deletePermission.failed"), { variant: "error" });
    }
  });

  const getRoleUsersList = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/${ctx.id}/users`,
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
    onPending: () => dispatch(actions.startGetUsersList()),
    onSuccess: (r) => dispatch(actions.finishGetUsersList(r.payload.payload)),
    onError: () => {
      dispatch(actions.finishGetUsersList(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.getUsers.failed"), { variant: "error" });
    }
  });

  const assignUserToRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/assign-user`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeleteUser()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeleteUser());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.assignUser.success"));
      if (r.ctx.fop) {
        getRoleUsersList({ id: r.ctx.request.roleId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeleteUser(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.assignUser.failed"), { variant: "error" });
    }
  });  

  const deleteUserFromRole = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      `${baseEndpoint}/delete-user`,
      ctx.request,
      { headers: { ...ahp() } }),
    onPending: () => dispatch(actions.startAssignDeleteUser()),
    onSuccess: (r) => {
      dispatch(actions.finishAssignDeleteUser());
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.deleteUser.success"));
      if (r.ctx.fop) {
        getRoleUsersList({ id: r.ctx.request.roleId, fop: r.ctx.fop });
      }
    },
    onError: () => {
      dispatch(actions.finishAssignDeletePermission(null));
      enqueueSnackbar(t("lp-ui-admin-panel:s.messages.roles.deleteUser.failed"), { variant: "error" });
    }
  });

  return {
    state: {
      list: useSelector((s) => s.apis.adminPanel.roles.list),
      single: useSelector((s) => s.apis.adminPanel.roles.single),
      create: useSelector((s) => s.apis.adminPanel.roles.create),
      update: useSelector((s) => s.apis.adminPanel.roles.update),
      deleteState: useSelector((s) => s.apis.adminPanel.roles.delete),
      permissionsList: useSelector((s) => s.apis.adminPanel.roles.permissionsList),
      assignDeletePermission: useSelector((s) => s.apis.adminPanel.roles.assignDeletePermission),
      usersList: useSelector((s) => s.apis.adminPanel.roles.usersList),
      assignDeleteUser: useSelector((s) => s.apis.adminPanel.roles.assignDeleteUser)

    },
    actions: {
      getRolesList,
      getRole,
      resetRole,
      createRole,
      updateRole,
      deleteRole,
      getRolePermissionsList,
      assignPermissionToRole,
      deletePermissionFromRole,
      getRoleUsersList,
      assignUserToRole,
      deleteUserFromRole
    }
  };
}