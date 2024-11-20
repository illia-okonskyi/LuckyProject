import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import TextField from "@mui/material/TextField";
import MenuItem from "@mui/material/MenuItem";
import IconButton from "@mui/material/IconButton";
import Select from "@mui/material/Select";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";

import { textColumn, customColumn } from "../../../../Common/LpDataList/columns";
import LpFopDataList from "../../../../Common/LpDataList/LpFopDataList";
import { useFiltersOrderPage } from "../../../../Common/LpDataList/useFiltersOrderPage";

import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelUsersApi } from "../../../../../features/apis/adminPanel/useAdminPanelUsersApi";

export default function AdminPanelUserPermissions() {
  const { userId } = useParams();
  const {
    state: { single: user, permissionsList },
    actions: { getUser, getUserPermissionsList }
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
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.observeUserPermissions", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/users",
        t("lp-ui-admin-panel:s.headersCrumbs.users")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.observeUserPermissions", { displayName }))
    ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, userId]);
  useEffect(() => {
    setCrumbData("userName", user?.userName);
  }, [user, setCrumbData]);

  useMountEffect(() => {
    getUser({ id: userId });
    getUserPermissionsList({ id: userId, fop: { filters: {}, order: "id-asc", page: 1 } });
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
    textColumn({
      id: 4,
      target: "fromRoleName",
      title: t("lp-ui-admin-panel:s.common.fromRole"),
      headerLeft: true
    }),
    customColumn({
      id: 5,
      target: null,
      title: t("lp-ui-admin-panel:s.common.value"),
      renderer: (_value, item) => {
        let strings = [t("lp-ui-admin-panel:s.common.root")];
        if (item.type === "Binary") {
          strings = [item.allow
            ? t("lp-ui-admin-panel:s.common.allow")
            : t("lp-ui-admin-panel:s.common.disaloow")
          ];
        } else if (item.type === "Level") {
          strings = [item.level];
        } else if (item.type === "Passkey") {
          strings = item.passkeys;
        }

        return (
          <Stack direction="row">
            {strings.map((s, i) => (<Typography
              key={i}
              variant="body1"
              color="secondary"
              textAlign="left">
              {s}
            </Typography>)
            )}
          </Stack>
        );
      }
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
            color="primary"
            onClick={() => navigateTo("admin-panel/permissions/details", { permissionId: item.id })}>
            <EditNoteRoundedIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigateTo("admin-panel/roles/details", { roleId: item.fromRoleId })}>
            <GroupRoundedIcon />
          </IconButton>
        </Stack>),
    }),
  ];

  const orders = {
    ["id-asc"]: `${t("lp-ui-admin-panel:s.common.id")} ↓`,
    ["id-desc"]: `${t("lp-ui-admin-panel:s.common.id")} ↑`,
    ["type-asc"]: `${t("lp-ui-admin-panel:s.common.type")} ↓`,
    ["type-desc"]: `${t("lp-ui-admin-panel:s.common.type")} ↑`,
    ["name-asc"]: `${t("lp-ui-admin-panel:s.common.name")} ↓`,
    ["name-desc"]: `${t("lp-ui-admin-panel:s.common.name")} ↑`,
    ["role-asc"]: `${t("lp-ui-admin-panel:s.common.role")} ↓`,
    ["role-desc"]: `${t("lp-ui-admin-panel:s.common.role")} ↑`,
  };
  const fop = useFiltersOrderPage({
    defaultOrder: Object.keys(orders)[0],
    defaultFilters: { type: "Any", name: "", role: "" }
  });

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "67%" }} textAlign="center">
      <LpFopDataList
        header={t("lp-ui-admin-panel:s.other.userPermissions")}
        items={permissionsList}
        columns={columns}
        fop={fop}
        onFopChanged={(fop) => getUserPermissionsList({ id: user.id, fop })}
        orders={orders}
        filterOrderDirection="column"
        renderFilters={(filters, setFilter) => (
          <Stack spacing={1} direction="row" alignItems="center">
            <FormControl>
              <InputLabel id="type-label">
                {t("lp-ui-admin-panel:Type")}
              </InputLabel>
              <Select
                size="small"
                labelId="type-label"
                label={t("lp-ui-admin-panel:Type")}
                value={filters.type}
                onChange={(e) => setFilter("type", e.target.value)}
              >
                <MenuItem value="Any">{t("lp-ui-admin-panel:s.common.any")}</MenuItem>
                <MenuItem value="Root">{t("lp-ui-admin-panel:s.common.root")}</MenuItem>
                <MenuItem value="Binary">{t("lp-ui-admin-panel:s.common.binary")}</MenuItem>
                <MenuItem value="Level">{t("lp-ui-admin-panel:s.common.level")}</MenuItem>
                <MenuItem value="Passkey">{t("lp-ui-admin-panel:s.common.passkey")}</MenuItem>
              </Select>
            </FormControl>
            <TextField
              size="small"
              label={t("lp-ui-admin-panel:s.common.name")}
              value={filters.name}
              onChange={(e) => setFilter("name", e.target.value)} />
            <TextField
              size="small"
              label={t("lp-ui-admin-panel:s.common.role")}
              value={filters.role}
              onChange={(e) => setFilter("role", e.target.value)} />
          </Stack>
        )}
      />
    </Stack>
  );
}
