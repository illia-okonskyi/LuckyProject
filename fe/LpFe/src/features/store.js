import { configureStore, combineReducers } from "@reduxjs/toolkit";

import { lpNavigationReducer } from "./navigation";
import { lpBreadcrumbsReducer } from "./breadcrumbs";
import { lpNotificationsReducer } from "./notifications";

import { authProfileReducer } from "./auth/useAuthProfile";

import { apisApiReducer } from "./apis/useApisApi";
import { adminPanelPermissionsApiReducer } from "./apis/adminPanel/useAdminPanelPermissionsApi";
import { adminPanelRolesApiReducer } from "./apis/adminPanel/useAdminPanelRolesApi";
import { adminPanelUsersApiReducer } from "./apis/adminPanel/useAdminPanelUsersApi";
import { adminPanelClientsApiReducer } from "./apis/adminPanel/useAdminPanelClientsApi";

const authReducer = combineReducers({
  profile: authProfileReducer
});

const adminPanelApiReducer = combineReducers({
  permissions: adminPanelPermissionsApiReducer,
  roles: adminPanelRolesApiReducer,
  users: adminPanelUsersApiReducer,
  clients: adminPanelClientsApiReducer
});

const apisReducer = combineReducers({
  api: apisApiReducer,
  adminPanel: adminPanelApiReducer
});

const rootReducer = combineReducers({
  navigation: lpNavigationReducer,
  breadcrumbs: lpBreadcrumbsReducer,
  notifications: lpNotificationsReducer,
  auth: authReducer,
  apis: apisReducer
});

const store = configureStore({ reducer: rootReducer });

export default store;