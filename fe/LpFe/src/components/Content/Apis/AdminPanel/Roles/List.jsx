import { useEffect } from "react";
import { useTranslation } from "react-i18next";

import Stack from "@mui/material/Stack";
import IconButton from "@mui/material/IconButton";
import Button from "@mui/material/Button";
import Divider from "@mui/material/Divider";

import LpLoadGuard from "../../../../Common/LpLoadGuard";
import { textColumn, customColumn } from "../../../../Common/LpDataList/columns";
import LpFopDataList from "../../../../Common/LpDataList/LpFopDataList";

import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import PolicyRoundedIcon from "@mui/icons-material/PolicyRounded";
import DeleteRoundedIcon from "@mui/icons-material/DeleteRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelRolesApi } from "../../../../../features/apis/adminPanel/useAdminPanelRolesApi";

import {
  rolesOrders,
  useRolesFiltersOrderPage,
  renderRolesFilters
} from "../Common/fop";

export default function AdminPanelRolesList() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { list, deleteState },
    actions: { getRolesList, deleteRole, resetRole }
  } = useAdminPanelRolesApi();
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.roles"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(1, null, t("lp-ui-admin-panel:s.headersCrumbs.roles"))
    ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  useMountEffect(() => {
    resetRole();
    getRolesList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const columns = [
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
      target: "isSealed",
      title: null,
      details: false,
      headerRight: true,
      renderer: (value, item, fop) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/roles/details", {roleId: item.id})}>
            <EditNoteRoundedIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/roles/users", { roleId: item.id })}>
            <GroupRoundedIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/roles/permissions", { roleId: item.id })}>
            <PolicyRoundedIcon />
          </IconButton>
          {(canManage && !value ) &&
            (<LpLoadGuard loading={deleteState.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deleteRole({ id: item.id, fop })}>
                <DeleteRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>)
    }),
  ];

  const orders = rolesOrders(t);
  const fop = useRolesFiltersOrderPage(orders);
  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.common.roles")}
        items={list}
        columns={columns}
        fop={fop}
        onFopChanged={(fop) => getRolesList({ fop })}
        orders={orders}
        renderFilters={(filters, setFilter) => renderRolesFilters(filters, setFilter, t)}
      />
      <Divider/>
      <Stack spacing={1} justifyContent="center" alignItems="center">
        <Button
          variant="contained"
          color="primary"
          onClick={() => navigateTo("admin-panel/roles/create")}>
          {t("lp-ui-admin-panel:s.headersCrumbs.createRole")}
        </Button>
      </Stack>
    </Stack>
  );
}
