import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import IconButton from "@mui/material/IconButton";
import Divider from "@mui/material/Divider";

import LpLoadGuard from "../../../../Common/LpLoadGuard";
import { textColumn, customColumn } from "../../../../Common/LpDataList/columns";
import LpFopDataList from "../../../../Common/LpDataList/LpFopDataList";

import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import AddCircleRoundedIcon from "@mui/icons-material/AddCircleRounded";
import RemoveCircleRoundedIcon from "@mui/icons-material/RemoveCircleRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelUsersApi } from "../../../../../features/apis/adminPanel/useAdminPanelUsersApi";
import { useAdminPanelRolesApi } from "../../../../../features/apis/adminPanel/useAdminPanelRolesApi";
import { useAuthProfile } from "../../../../../features/auth/useAuthProfile";

import {
  usersOrders,
  useUsersFiltersOrderPage,
  renderUsersFilters
} from "../Common/fop";

export default function AdminPanelRoleUsers() {
  const { hasApiFeature } = useAppAuthStore();
  const { roleId } = useParams();
  const {
    state: { list: usersList },
    actions: { getUsersList }
  } = useAdminPanelUsersApi();
  const {
    state: { single: role, usersList: roleUsersList, assignDeleteUser },
    actions: {
      getRole,
      getRoleUsersList,
      assignUserToRole,
      deleteUserFromRole
    }
  } = useAdminPanelRolesApi();
  const {
    state: { locales },
    actions: { getLocales, }
  } = useAuthProfile();  
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const {
    setCrumbs,
    buildCrumb,
    setData: setCrumbData,
    useData: useCrumbData
  } = useLpBreadcrumbs();
  const crumbData = useCrumbData();

  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    const displayName = crumbData.roleName || roleId;
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageRoleUsers", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/roles",
        t("lp-ui-admin-panel:s.headersCrumbs.roles")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageRoleUsers", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, roleId]);
  useEffect(() => {
    setCrumbData("roleName", role?.name);
  }, [role, setCrumbData]);

  useMountEffect(() => {
    getRole({ id: roleId });
    getLocales();
    getRoleUsersList({ id: roleId, fop: { filters: {}, order: "id-asc", page: 1 }});
    getUsersList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  const canEdit = canManage && (!(role?.isSealed || false));

  const orders = usersOrders(t);
  const roleUsersFop = useUsersFiltersOrderPage(orders); 
  const usersFop = useUsersFiltersOrderPage(orders); 

  const roleUsersColumns = [
    textColumn({
      id: 0,
      target: "userName",
      title: t("lp-ui-admin-panel:s.common.userName"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "email",
      title: t("lp-ui-admin-panel:s.common.email")
    }),
    textColumn({
      id: 2,
      target: "phoneNumber",
      title: t("lp-ui-admin-panel:s.common.phoneNumber")
    }),
    textColumn({
      id: 3,
      target: "fullName",
      title: t("lp-ui-admin-panel:s.common.fullName"),
      headerLeft: true
    }),
    textColumn({
      id: 4,
      target: "telegramUserName",
      title: t("lp-ui-admin-panel:s.common.tgUserName")
    }),
    textColumn({
      id: 5,
      target: "preferredLocaleDisplayName",
      title: t("lp-ui-admin-panel:s.common.preferredLocale")
    }),
    customColumn({
      id: 6,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/users/details", { userId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          {canEdit &&
            (<LpLoadGuard loading={assignDeleteUser.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deleteUserFromRole({
                  request: { roleId: role.id, userId: item.id},
                  fop: roleUsersFop
                  })}>
                <RemoveCircleRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>),
    }),
  ];

  const usersColumns = [
    textColumn({
      id: 0,
      target: "userName",
      title: t("lp-ui-admin-panel:s.common.userName"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "email",
      title: t("lp-ui-admin-panel:s.common.email")
    }),
    textColumn({
      id: 2,
      target: "phoneNumber",
      title: t("lp-ui-admin-panel:s.common.phoneNumber")
    }),
    textColumn({
      id: 3,
      target: "fullName",
      title: t("lp-ui-admin-panel:s.common.fullName"),
      headerLeft: true
    }),
    textColumn({
      id: 4,
      target: "telegramUserName",
      title: t("lp-ui-admin-panel:s.common.tgUserName")
    }),
    textColumn({
      id: 5,
      target: "preferredLocaleDisplayName",
      title: t("lp-ui-admin-panel:s.common.preferredLocale")
    }),
    customColumn({
      id: 6,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/users/details", { userId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          <LpLoadGuard loading={assignDeleteUser.pending}>
            <IconButton
              size="small"
              color="primary"
              onClick={() => assignUserToRole({
                request: { roleId: role.id, userId: item.id},
                fop: roleUsersFop
                })}>
              <AddCircleRoundedIcon />
            </IconButton>
          </LpLoadGuard>
        </Stack>)
    }),
  ];

  let child = null;
  if (!canManage) {
    child = (<Typography variant="body1" color="error">{t("lp-ui-admin-panel:s.common.accessDenied")}</Typography>);
  }
  else {
    const renderFilters = (filters, setFilter) => renderUsersFilters(filters, setFilter, locales, t);
    child = (<>
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.other.assignedUsers")}
        items={roleUsersList}
        columns={roleUsersColumns}
        fop={roleUsersFop}
        onFopChanged={(fop) => getRoleUsersList({ id: roleId, fop})}
        orders={orders}
        renderFilters={renderFilters}
        filterOrderDirection="column"
      />
      {canEdit && (<>
        <Divider/>
        <LpFopDataList
          header={t("lp-ui-admin-panel:s.common.users")}
          items={usersList}
          columns={usersColumns}
          fop={usersFop}
          onFopChanged={(fop) => getUsersList({ fop })}
          orders={orders}
          renderFilters={renderFilters}
          filterOrderDirection="column"
      />      
      </>)}
    </>);
  }

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      {child}
    </Stack>
  );
}
