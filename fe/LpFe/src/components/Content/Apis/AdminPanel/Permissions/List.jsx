import { useEffect } from "react";
import { useTranslation } from "react-i18next";

import Stack from "@mui/material/Stack";
import IconButton from "@mui/material/IconButton";

import LpLoadGuard from "../../../../Common/LpLoadGuard";
import { textColumn, customColumn } from "../../../../Common/LpDataList/columns";
import LpFopDataList from "../../../../Common/LpDataList/LpFopDataList";

import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import DeleteRoundedIcon from "@mui/icons-material/DeleteRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelPermissionsApi } from "../../../../../features/apis/adminPanel/useAdminPanelPermissionsApi";

import {
  permissionsOrders,
  usePermissionsFiltersOrderPage,
  renderPermissionsFilters
} from "../Common/fop";

export default function AdminPanelPermissionsList() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { list, deleteState },
    actions: { getPermissionsList, deletePermission }
  } = useAdminPanelPermissionsApi();
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.permissions"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(1, null, t("lp-ui-admin-panel:s.headersCrumbs.permissions"))
    ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  useMountEffect(() => {
    getPermissionsList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const columns = [
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
      target: "isSealed",
      title: null,
      details: false,
      headerRight: true,
      renderer: (value, item, fop) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/permissions/details", {permissionId: item.id})}>
            <EditNoteRoundedIcon />
          </IconButton>
          {(canManage && !value ) &&
            (<LpLoadGuard loading={deleteState.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deletePermission({ id: item.id, fop })}>
                <DeleteRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>)
    }),
  ];

  const orders = permissionsOrders(t);
  const fop = usePermissionsFiltersOrderPage(orders);
  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.common.permissions")}
        items={list}
        columns={columns}
        fop={fop}
        onFopChanged={(fop) => getPermissionsList({ fop })}
        orders={orders}
        renderFilters={(filters, setFilter) => renderPermissionsFilters(filters, setFilter, t)}
      />
      </Stack>
  );
}
