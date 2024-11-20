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

import { useAdminPanelPermissionsApi } from "../../../../../features/apis/adminPanel/useAdminPanelPermissionsApi";
import { useAdminPanelRolesApi } from "../../../../../features/apis/adminPanel/useAdminPanelRolesApi";

import {
  permissionsOrders,
  usePermissionsFiltersOrderPage,
  renderPermissionsFilters
} from "../Common/fop";

export default function AdminPanelRolePermissions() {
  const { hasApiFeature } = useAppAuthStore();
  const { roleId } = useParams();
  const {
    state: { list: permissionsList },
    actions: { getPermissionsList }
  } = useAdminPanelPermissionsApi();
  const {
    state: { single: role, permissionsList: rolePermissionsList, assignDeletePermission },
    actions: {
      getRole,
      getRolePermissionsList,
      assignPermissionToRole,
      deletePermissionFromRole
    }
  } = useAdminPanelRolesApi();
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
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageRolePermissions", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/roles",
        t("lp-ui-admin-panel:s.headersCrumbs.roles")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageRolePermissions", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, roleId]);
  useEffect(() => {
    setCrumbData("roleName", role?.name);
  }, [role, setCrumbData]);

  useMountEffect(() => {
    getRole({ id: roleId });
    getRolePermissionsList({ id: roleId, fop: { filters: {}, order: "id-asc", page: 1 }});
    getPermissionsList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  const canEdit = canManage && (!(role?.isSealed || false));

  const orders = permissionsOrders(t);
  const rolePermissionsFop = usePermissionsFiltersOrderPage(orders);
  const permissionsFop = usePermissionsFiltersOrderPage(orders);

  const rolePermissionsColumns = [
    textColumn({
      id: 0,
      target: "type",
      title: t("lp-ui-admin-panel:s.common.type"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "name",
      title: t("lp-ui-admin-panel:s.common.name"),
      headerLeft: true
    }),
    textColumn({
      id: 2,
      target: "fullName",
      title: t("lp-ui-admin-panel:s.common.fullName")
    }),
    textColumn({
      id: 3,
      target: "description",
      title: t("lp-ui-admin-panel:s.common.description")
    }),
    customColumn({
      id: 4,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/permissions/details", { permissionId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          {canEdit &&
            (<LpLoadGuard loading={assignDeletePermission.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deletePermissionFromRole({
                  request: { roleId: role.id, permissionId: item.id},
                  fop: rolePermissionsFop
                  })}>
                <RemoveCircleRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>),
    }),
  ];

  const permissionsColumns = [
    textColumn({
      id: 0,
      target: "type",
      title: t("lp-ui-admin-panel:s.common.type"),
      headerLeft: true
    }),
    textColumn({
      id: 1,
      target: "name",
      title: t("lp-ui-admin-panel:s.common.name"),
      headerLeft: true
    }),
    textColumn({
      id: 2,
      target: "fullName",
      title: t("lp-ui-admin-panel:s.common.fullName")
    }),
    textColumn({
      id: 3,
      target: "description",
      title: t("lp-ui-admin-panel:s.common.description")
    }),
    customColumn({
      id: 4,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="secondary"
            onClick={() => navigateTo("admin-panel/permissions/details", { permissionId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          <LpLoadGuard loading={assignDeletePermission.pending}>
            <IconButton
              size="small"
              color="primary"
              onClick={() => assignPermissionToRole({
                request: { roleId: role.id, permissionId: item.id},
                fop: rolePermissionsFop
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
    const renderFilters = (filters, setFilter) => renderPermissionsFilters(filters, setFilter, t);
    child = (<>
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.other.assignedPermissions")}
        items={rolePermissionsList}
        columns={rolePermissionsColumns}
        fop={rolePermissionsFop}
        onFopChanged={(fop) => getRolePermissionsList({ id: roleId, fop})}
        orders={orders}
        renderFilters={renderFilters}
      />
      {canEdit && (<>
        <Divider/>
        <LpFopDataList
          header={t("lp-ui-admin-panel:s.common.permissions")}
          items={permissionsList}
          columns={permissionsColumns}
          fop={permissionsFop}
          onFopChanged={(fop) => getPermissionsList({ fop })}
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
