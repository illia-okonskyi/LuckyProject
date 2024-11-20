import { useFiltersOrderPage } from "../../../../Common/LpDataList/useFiltersOrderPage";

import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";

//#region Permissions
export function permissionsOrders(t) {
  return {
    ["id-asc"]: `${t("lp-ui-admin-panel:s.common.id")} ↓`,
    ["id-desc"]: `${t("lp-ui-admin-panel:s.common.id")} ↑`,
    ["type-asc"]: `${t("lp-ui-admin-panel:s.common.type")} ↓`,
    ["type-desc"]: `${t("lp-ui-admin-panel:s.common.type")} ↑`,
    ["name-asc"]: `${t("lp-ui-admin-panel:s.common.name")} ↓`,
    ["name-desc"]: `${t("lp-ui-admin-panel:s.common.name")} ↑`,
  };
}

export function usePermissionsFiltersOrderPage(orders) {
  return useFiltersOrderPage({
    defaultOrder: Object.keys(orders)[0],
    defaultFilters: { type: "Any", name: "" }
  });
}

export function renderPermissionsFilters(filters, setFilter, t) {
  return (
    <Stack spacing={1} direction="row" alignItems="center">
      <FormControl>
        <InputLabel id="type-label">
          {t("lp-ui-admin-panel:s.common.type")}
        </InputLabel>
        <Select
          size="small"
          labelId="type-label"
          label={t("lp-ui-admin-panel:s.common.type")}
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
    </Stack>
  );
}
//#endregion

//#region Roles
export function rolesOrders(t) {
  return {
    ["id-asc"]: `${t("lp-ui-admin-panel:s.common.id")} ↓`,
    ["id-desc"]: `${t("lp-ui-admin-panel:s.common.id")} ↑`,
    ["name-asc"]: `${t("lp-ui-admin-panel:s.common.name")} ↓`,
    ["name-desc"]: `${t("lp-ui-admin-panel:s.common.name")} ↑`,
  };
}

export function useRolesFiltersOrderPage(orders) {
  return useFiltersOrderPage({
    defaultOrder: Object.keys(orders)[0],
    defaultFilters: { name: "" }
  });
}

export function renderRolesFilters(filters, setFilter, t) {
  return (
    <Stack spacing={1} direction="row" alignItems="center">
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.name")}
        value={filters.name}
        onChange={(e) => setFilter("name", e.target.value)} />
    </Stack>
  );
}
//#endregion

//#region Users
export function usersOrders(t) {
  return {
    ["id-asc"]: `${t("lp-ui-admin-panel:s.common.id")} ↓`,
    ["id-desc"]: `${t("lp-ui-admin-panel:s.common.id")} ↑`,
    ["userName-asc"]: `${t("lp-ui-admin-panel:s.common.userName")} ↓`,
    ["userName-desc"]: `${t("lp-ui-admin-panel:s.common.userName")} ↑`,
    ["email-asc"]: `${t("lp-ui-admin-panel:s.common.email")} ↓`,
    ["email-desc"]: `${t("lp-ui-admin-panel:s.common.email")} ↑`,
    ["phoneNumber-asc"]: `${t("lp-ui-admin-panel:s.common.phoneNumber")} ↓`,
    ["phoneNumber-desc"]: `${t("lp-ui-admin-panel:s.common.phoneNumber")} ↑`,
    ["fullName-asc"]: `${t("lp-ui-admin-panel:s.common.fullName")} ↓`,
    ["fullName-desc"]: `${t("lp-ui-admin-panel:s.common.fullName")} ↑`,
    ["tgUserName-asc"]: `${t("lp-ui-admin-panel:s.common.tgUserName")} ↓`,
    ["tgUserName-desc"]: `${t("lp-ui-admin-panel:s.common.tgUserName")} ↑`,
    ["preferredLocale-asc"]: `${t("lp-ui-admin-panel:s.common.preferredLocale")} ↓`,
    ["preferredLocale-desc"]: `${t("lp-ui-admin-panel:s.common.preferredLocale")} ↑`,
  };
}

export function useUsersFiltersOrderPage(orders) {
  return useFiltersOrderPage({
    defaultOrder: Object.keys(orders)[0],
    defaultFilters: {
      userName: "",
      email: "",
      phoneNumber: "",
      fullName: "",
      tgUserName: "",
      preferredLocale: "Any"
    }
  });
}

export function renderUsersFilters(filters, setFilter, locales, t) {
  return (
    <Stack spacing={1}>
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.userName")}
        value={filters.userName}
        onChange={(e) => setFilter("userName", e.target.value)} />
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.email")}
        value={filters.email}
        onChange={(e) => setFilter("email", e.target.value)} />
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.phoneNumber")}
        value={filters.phoneNumber}
        onChange={(e) => setFilter("phoneNumber", e.target.value)} />
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.fullName")}
        value={filters.fullName}
        onChange={(e) => setFilter("fullName", e.target.value)} />
      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.tgUserName")}
        value={filters.tgUserName}
        onChange={(e) => setFilter("tgUserName", e.target.value)} />
      {locales.value && (
        <FormControl>
          <InputLabel id="preferredLocale-label">
            {t("lp-ui-admin-panel:s.common.preferredLocale")}
          </InputLabel>
          <Select
            size="small"
            labelId="preferredLocale-label"
            label={t("lp-ui-admin-panel:s.common.preferredLocale")}
            value={filters.preferredLocale}
            onChange={(e) => setFilter("preferredLocale", e.target.value)}
          >
            <MenuItem key="Any" value="Any">{t("lp-ui-admin-panel:s.common.any")}</MenuItem>
            {locales.value?.map((l) => (<MenuItem key={l.name} value={l.name}>{l.displayName}</MenuItem>))}
          </Select>
        </FormControl>
      )}
    </Stack>
  );
}
//#endregion

//#region Clients
export function clientsOrders(t) {
  return {
    ["id-asc"]: `${t("lp-ui-admin-panel:s.common.id")} ↓`,
    ["id-desc"]: `${t("lp-ui-admin-panel:s.common.id")} ↑`,
    ["type-asc"]: `${t("lp-ui-admin-panel:s.common.type")} ↓`,
    ["type-desc"]: `${t("lp-ui-admin-panel:s.common.type")} ↑`,
    ["name-asc"]: `${t("lp-ui-admin-panel:s.common.name")} ↓`,
    ["name-desc"]: `${t("lp-ui-admin-panel:s.common.name")} ↑`,
  };
}

export function useClientsFiltersOrderPage(orders) {
  return useFiltersOrderPage({
    defaultOrder: Object.keys(orders)[0],
    defaultFilters: {
      type: "Any",
      name: ""
    }
  });
}

export function renderClientsFilters(filters, setFilter, t) {
  return (
    <Stack spacing={1} direction="row" alignItems="center">
      <FormControl>
        <InputLabel id="type-label">
          {t("lp-ui-admin-panel:s.common.type")}
        </InputLabel>
        <Select
          size="small"
          labelId="type-label"
          label={t("lp-ui-admin-panel:s.common.type")}
          value={filters.type}
          onChange={(e) => setFilter("type", e.target.value)}
        >
          <MenuItem value="Any">{t("lp-ui-admin-panel:s.common.any")}</MenuItem>
          <MenuItem value="Web">{t("lp-ui-admin-panel:s.common.web")}</MenuItem>
          <MenuItem value="Machine">{t("lp-ui-admin-panel:s.common.machine")}</MenuItem>
        </Select>
      </FormControl>

      <TextField
        size="small"
        label={t("lp-ui-admin-panel:s.common.name")}
        value={filters.name}
        onChange={(e) => setFilter("name", e.target.value)} />
    </Stack>
  );
}
//#endregion