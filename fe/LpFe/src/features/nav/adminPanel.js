import AdminPanelPermissionsList from "../../components/Content/Apis/AdminPanel/Permissions/List";
import AdminPanelPermissionDetails from "../../components/Content/Apis/AdminPanel/Permissions/Details";

import AdminPanelRolesList from "../../components/Content/Apis/AdminPanel/Roles/List";
import AdminPanelCreateRole from "../../components/Content/Apis/AdminPanel/Roles/Create";
import AdminPanelRoleDetails from "../../components/Content/Apis/AdminPanel/Roles/Details";
import AdminPanelRolePermissions from "../../components/Content/Apis/AdminPanel/Roles/Permissions";
import AdminPanelRoleUsers from "../../components/Content/Apis/AdminPanel/Roles/Users";

import AdminPanelUsersList from "../../components/Content/Apis/AdminPanel/Users/List";
import AdminPanelCreateUser from "../../components/Content/Apis/AdminPanel/Users/Create";
import AdminPanelUserDetails from "../../components/Content/Apis/AdminPanel/Users/Details";
import AdminPanelUserRoles from "../../components/Content/Apis/AdminPanel/Users/Roles";
import AdminPanelUserPermissions from "../../components/Content/Apis/AdminPanel/Users/Permissions";

import AdminPanelClientsList from "../../components/Content/Apis/AdminPanel/Clients/List";
import AdminPanelCreateWebClient from "../../components/Content/Apis/AdminPanel/Clients/CreateWeb";
import AdminPanelCreateMachineClient from "../../components/Content/Apis/AdminPanel/Clients/CreateMachine";
import AdminPanelClientDetails from "../../components/Content/Apis/AdminPanel/Clients/Details";

import AdminPanelSettingsRoundedIcon from "@mui/icons-material/AdminPanelSettingsRounded";
import PolicyRoundedIcon from "@mui/icons-material/PolicyRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import CloudCircleRoundedIcon from "@mui/icons-material/CloudCircleRounded";

export function navItemsAdminPanelApiMetadataProvider(id) {
  if (id === "admin-panel") {
    return {
      toBuilder: null,
      component: null,
      navIcon: AdminPanelSettingsRoundedIcon
    };
  }

  if (id === "admin-panel/permissions") {
    return {
      toBuilder: () => "/admin-panel/permissions",
      component: AdminPanelPermissionsList,
      navIcon: PolicyRoundedIcon
    };
  }

  if (id === "admin-panel/permissions/details") {
    return {
      toBuilder: (params) => `/admin-panel/permissions/${params.permissionId}`,
      component: AdminPanelPermissionDetails,
      navIcon: null
    };
  }

  if (id === "admin-panel/roles") {
    return {
      toBuilder: () => "/admin-panel/roles",
      component: AdminPanelRolesList,
      navIcon: GroupRoundedIcon
    };
  }

  if (id === "admin-panel/roles/create") {
    return {
      toBuilder: () => "/admin-panel/roles/create",
      component: AdminPanelCreateRole,
      navIcon: null
    };
  }

  if (id === "admin-panel/roles/details") {
    return {
      toBuilder: (params) => `/admin-panel/roles/${params.roleId}`,
      component: AdminPanelRoleDetails,
      navIcon: null
    };
  }

  if (id === "admin-panel/roles/permissions") {
    return {
      toBuilder: (params) => `/admin-panel/roles/${params.roleId}/permissions`,
      component: AdminPanelRolePermissions,
      navIcon: null
    };
  }

  if (id === "admin-panel/roles/users") {
    return {
      toBuilder: (params) => `/admin-panel/roles/${params.roleId}/users`,
      component: AdminPanelRoleUsers,
      navIcon: null
    };
  }

  if (id === "admin-panel/users") {
    return {
      toBuilder: () => "/admin-panel/users",
      component: AdminPanelUsersList,
      navIcon: GroupRoundedIcon
    };
  }

  if (id === "admin-panel/users/create") {
    return {
      toBuilder: () => "/admin-panel/users/create",
      component: AdminPanelCreateUser,
      navIcon: null
    };
  }

  if (id === "admin-panel/users/details") {
    return {
      toBuilder: (params) => `/admin-panel/users/${params.userId}`,
      component: AdminPanelUserDetails,
      navIcon: null
    };
  }

  if (id === "admin-panel/users/roles") {
    return {
      toBuilder: (params) => `/admin-panel/users/${params.userId}/roles`,
      component: AdminPanelUserRoles,
      navIcon: null
    };
  }

  if (id === "admin-panel/users/permissions") {
    return {
      toBuilder: (params) => `/admin-panel/users/${params.userId}/permissions`,
      component: AdminPanelUserPermissions,
      navIcon: null
    };
  }

  if (id === "admin-panel/clients") {
    return {
      toBuilder: () => "/admin-panel/clients",
      component: AdminPanelClientsList,
      navIcon: CloudCircleRoundedIcon
    };
  }

  if (id === "admin-panel/clients/create-web") {
    return {
      toBuilder: () => "/admin-panel/clients/create-web",
      component: AdminPanelCreateWebClient,
      navIcon: null
    };
  }

  if (id === "admin-panel/clients/create-machine") {
    return {
      toBuilder: () => "/admin-panel/clients/create-machine",
      component: AdminPanelCreateMachineClient,
      navIcon: null
    };
  }

  if (id === "admin-panel/clients/details") {
    return {
      toBuilder: (params) => `/admin-panel/clients/${params.clientId}`,
      component: AdminPanelClientDetails,
      navIcon: null
    };
  }

  return null;
}

export function navItemsAdminPanelApiMatcher(api) {
  if (api.name !== "Admin-Panel") {
    return null;
  }

  return {
    id: "admin-panel",
    navIndex: 2,
    navHeaderI18nKey: "lp-ui-shell:s.nav.adminPanel",
    route: null,
    childs: [{
      id: "admin-panel/permissions",
      navIndex: 0,
      navHeaderI18nKey: "lp-ui-shell:s.nav.adminPanel.permissions",
      route: "/admin-panel/permissions",
      childs: [{
        id: "admin-panel/permissions/details",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/permissions/:permissionId",
        childs: []
      }]
    }, {
      id: "admin-panel/roles",
      navIndex: 0,
      navHeaderI18nKey: "lp-ui-shell:s.nav.adminPanel.roles",
      route: "/admin-panel/roles",
      childs: [{
        id: "admin-panel/roles/create",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/roles/create",
        childs: []
      }, {
        id: "admin-panel/roles/details",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/roles/:roleId",
        childs: []
      }, {
        id: "admin-panel/roles/users",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/roles/:roleId/users",
        childs: []
      }, {
        id: "admin-panel/roles/permissions",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/roles/:roleId/permissions",
        childs: []
      }]
    }, {
      id: "admin-panel/users",
      navIndex: 0,
      navHeaderI18nKey: "lp-ui-shell:s.nav.adminPanel.users",
      route: "/admin-panel/users",
      childs: [{
        id: "admin-panel/users/create",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/users/create",
        childs: []
      }, {
        id: "admin-panel/users/details",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/users/:userId",
        childs: []
      }, {
        id: "admin-panel/users/roles",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/users/:userId/roles",
        childs: []
      }, {
        id: "admin-panel/users/permissions",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/users/:userId/permissions",
        childs: []
      }]
    }, {
      id: "admin-panel/clients",
      navIndex: 0,
      navHeaderI18nKey: "lp-ui-shell:s.nav.adminPanel.clients",
      route: "/admin-panel/clients",
      childs: [{
        id: "admin-panel/clients/create-web",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/clients/create-web",
        childs: []
      }, {
        id: "admin-panel/clients/create-machine",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/clients/create-machine",
        childs: []
      }, {
        id: "admin-panel/clients/details",
        navIndex: 0,
        navHeaderI18nKey: null,
        route: "admin-panel/clients/:clientId",
        childs: []
      }]
    }]
  };
}
