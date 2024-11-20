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

import { useAdminPanelUsersApi } from "../../../../../features/apis/adminPanel/useAdminPanelUsersApi";
import { useAuthProfile } from "../../../../../features/auth/useAuthProfile";

import {
  usersOrders,
  useUsersFiltersOrderPage,
  renderUsersFilters
} from "../Common/fop";

export default function AdminPanelUsersList() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { list, deleteState },
    actions: { getUsersList, deleteUser, resetUser }
  } = useAdminPanelUsersApi();
  const {
    state: { locales },
    actions: { getLocales, }
  } = useAuthProfile();  
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.users"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(1, null, t("lp-ui-admin-panel:s.headersCrumbs.users"))
    ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  useMountEffect(() => {
    resetUser();
    getLocales();
    getUsersList({ fop: { filters: {}, order: "id-asc", page: 1 }});
  });

  const columns = [
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
      target: "isSealed",
      title: null,
      details: false,
      headerRight: true,
      renderer: (value, item, fop) => (
        <Stack direction="row">
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/users/details", { userId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/users/roles", { userId: item.id })}>
            <GroupRoundedIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/users/permissions", { userId: item.id })}>
            <PolicyRoundedIcon />
          </IconButton>
          {(canManage && !value && !item.isMachineUser) &&
            (<LpLoadGuard loading={deleteState.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deleteUser({ id: item.id, fop })}>
                <DeleteRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>)
    }),
  ];

  const orders = usersOrders(t);
  const fop = useUsersFiltersOrderPage(orders);
  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.common.users")}
        items={list}
        columns={columns}
        fop={fop}
        onFopChanged={(fop) => getUsersList({ fop })}
        orders={orders}
        filterOrderDirection="column"
        renderFilters={(filters, setFilter) => renderUsersFilters(filters, setFilter, locales, t)}
      />
      <Divider/>
      <Stack spacing={1} justifyContent="center" alignItems="center">
        <Button
          variant="contained"
          color="primary"
          onClick={() => navigateTo("admin-panel/users/create")}>
          {t("lp-ui-admin-panel:s.headersCrumbs.createUser")}
        </Button>
      </Stack>
    </Stack>
  );
}
