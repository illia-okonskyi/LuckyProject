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

import {
  rolesOrders,
  useRolesFiltersOrderPage,
  renderRolesFilters
} from "../Common/fop";

export default function AdminPanelUserRoles() {
  const { hasApiFeature } = useAppAuthStore();
  const { userId } = useParams();
  const {
    state: { list: rolesList },
    actions: { getRolesList }
  } = useAdminPanelRolesApi();
  const {
    state: { single: user, rolesList: userRolesList, assignDeleteRole },
    actions: {
      getUser,
      getUserRolesList,
      assignRoleToUser,
      deleteRoleFromUser
    }
  } = useAdminPanelUsersApi();
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
    const displayName = crumbData.userName || userId;
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageUserRoles", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/users",
        t("lp-ui-admin-panel:s.headersCrumbs.users")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageUserRoles", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, userId]);
  useEffect(() => {
    setCrumbData("userName", user?.userName);
  }, [user, setCrumbData]);

  useMountEffect(() => {
    getUser({ id: userId });
    getUserRolesList({ id: userId, fop: { filters: {}, order: "id-asc", page: 1 }});
    getRolesList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  const canEdit = canManage && (!(user?.isSealed || false));

  const orders = rolesOrders(t);
  const userRolesFop = useRolesFiltersOrderPage(orders); 
  const rolesFop = useRolesFiltersOrderPage(orders); 

  const roleUsersColumns = [
    textColumn({
      id: 0,
      target: "name",
      title: t("lp-ui-admin-panel:s.common.name"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "description",
      title: t("lp-ui-admin-panel:s.common.description")
    }),
    customColumn({
      id: 2,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/roles/details", { roleId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          {canEdit &&
            (<LpLoadGuard loading={assignDeleteRole.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deleteRoleFromUser({
                  request: { userId: user.id, roleId: item.id},
                  fop: userRolesFop
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
      target: "name",
      title: t("lp-ui-admin-panel:s.common.name"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "description",
      title: t("lp-ui-admin-panel:s.common.description")
    }),
    customColumn({
      id: 2,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/roles/details", { roleId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          <LpLoadGuard loading={assignDeleteRole.pending}>
            <IconButton
              size="small"
              color="primary"
              onClick={() => assignRoleToUser({
                request: { userId: user.id, roleId: item.id},
                fop: userRolesFop
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
    const renderFilters = (filters, setFilter) => renderRolesFilters(filters, setFilter, t);
    child = (<>
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.other.assignedRoles")}
        items={userRolesList}
        columns={roleUsersColumns}
        fop={userRolesFop}
        onFopChanged={(fop) => getUserRolesList({ id: userId, fop})}
        orders={orders}
        renderFilters={renderFilters}
      />
      {canEdit && (<>
        <Divider/>
        <LpFopDataList
          header={t("lp-ui-admin-panel:s.common.roles")}
          items={rolesList}
          columns={usersColumns}
          fop={rolesFop}
          onFopChanged={(fop) => getRolesList({ fop })}
          orders={orders}
          renderFilters={renderFilters}
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
