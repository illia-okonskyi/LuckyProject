import { useEffect } from "react";
import { useTranslation } from "react-i18next";

import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import IconButton from "@mui/material/IconButton";
import Button from "@mui/material/Button";
import Divider from "@mui/material/Divider";

import LpLoadGuard from "../../../../Common/LpLoadGuard";
import { textColumn, customColumn } from "../../../../Common/LpDataList/columns";
import LpFopDataList from "../../../../Common/LpDataList/LpFopDataList";

import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import PersonRoundedIcon from "@mui/icons-material/PersonRounded";
import PolicyRoundedIcon from "@mui/icons-material/PolicyRounded";
import DeleteRoundedIcon from "@mui/icons-material/DeleteRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelClientsApi } from "../../../../../features/apis/adminPanel/useAdminPanelClientsApi";

import {
  clientsOrders,
  useClientsFiltersOrderPage,
  renderClientsFilters
} from "../Common/fop";

export default function AdminPanelClientsList() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { list, deleteState },
    actions: { getClientsList, deleteClient, resetClient }
  } = useAdminPanelClientsApi();
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.clients"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(1, null, t("lp-ui-admin-panel:s.headersCrumbs.clients"))
    ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  useMountEffect(() => {
    resetClient();
    getClientsList({ fop: { filters: {}, order: "id-asc", page: 1 }});
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
      target: "clientId",
      title: t("lp-ui-admin-panel:s.common.clientId")
    }),
    textColumn({
      id: 3,
      target: "displayName",
      title: t("lp-ui-admin-panel:s.common.displayName")
    }),
    customColumn({
      id: 4,
      target: "web",
      title: t("lp-ui-admin-panel:s.common.web"),
      details: true,
      renderer: (value) => {
        if (!value) {
          return null;
        }
        return (
          <Stack spacing={1} sx={{ textAlign: "left" }}>
            {value.origins.map((o, i) => (
              <Stack key={i}>
                <Typography variant="body1" color="secondary">{o.baseUrl}</Typography>
                <Stack padding={1}>
                  <Typography variant="body2" color="info">- {o.redirectUrl}</Typography>
                  <Typography variant="body2" color="info">- {o.postLogoutRedirectUrl}</Typography>
                </Stack>
              </Stack>
            ))}
          </Stack>);
      }
    }),
    customColumn({
      id: 5,
      target: "machine",
      title: t("lp-ui-admin-panel:s.common.machine"),
      details: true,
      renderer: (value) => {
        if (!value) {
          return null;
        }
        return (
          <Stack sx={{ textAlign: "left" }}>
            {value.origins.map((o, i) => (
              <Typography key={i} variant="body1" color="secondary">{o}</Typography>
            ))}
          </Stack>);
      }
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
            onClick={() => navigateTo("admin-panel/clients/details", { clientId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          { item.machine && (<>
            <IconButton
              size="small"
              color="primary"
              onClick={() => navigateTo("admin-panel/users/details", { userId: item.machine.userId })}>
              <PersonRoundedIcon />
            </IconButton>
            <IconButton
              size="small"
              color="primary"
              onClick={() => navigateTo("admin-panel/users/permissions", { userId: item.machine.userId })}>
              <PolicyRoundedIcon />
            </IconButton>
          </>)}
          {(canManage && !value) &&
            (<LpLoadGuard loading={deleteState.pending}>
              <IconButton
                size="small"
                color="error"
                onClick={() => deleteClient({ id: item.id, fop })}>
                <DeleteRoundedIcon />
              </IconButton>
            </LpLoadGuard>)}
        </Stack>)
    }),
  ];

  const orders = clientsOrders(t);
  const fop = useClientsFiltersOrderPage(orders);
  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.common.clients")}
        items={list}
        columns={columns}
        fop={fop}
        onFopChanged={(fop) => getClientsList({ fop })}
        orders={orders}
        filterOrderDirection="column"
        renderFilters={(filters, setFilter) => renderClientsFilters(filters, setFilter, t)}
      />
      <Divider/>
      <Stack spacing={1} width={{ xs: "100%", md: "50%" }} alignSelf="center">
        <Button
          variant="contained"
          color="primary"
          onClick={() => navigateTo("admin-panel/clients/create-web")}>
          {t("lp-ui-admin-panel:s.headersCrumbs.createWebClient")}
        </Button>
        <Button
          variant="contained"
          color="primary"
          onClick={() => navigateTo("admin-panel/clients/create-machine")}>
          {t("lp-ui-admin-panel:s.headersCrumbs.createMachineClient")}
        </Button>
      </Stack>
    </Stack>
  );
}
