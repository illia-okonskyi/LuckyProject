import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useForm, Controller } from "react-hook-form";

import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelRolesApi } from "../../../../../features/apis/adminPanel/useAdminPanelRolesApi";
import { useMountEffect } from "../../../../../hooks/useMountEffect";

function CreateRoleForm({ create, createRole, t}) {
  const { handleSubmit, control } = useForm({
    values: {
      name: "",
      description: "",
    },
  });

  const onSubmit = (data) => {
    createRole({ request: {
      name: data.name,
      description: data.description
    }});
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <Controller
        name="name"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.name")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationFieldError validation={create.validation} vkey="NormalizedName" />

      <Controller
        name="description"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.description")}
            size="small"
            multiline
            maxRows={4}
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationFieldError validation={create.validation} vkey="Description" />

      <Stack spacing={1} direction="row">
        <LpLoadGuard loading={create.pending}>
          <Button variant="contained" onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-admin-panel:s.common.create")}
          </Button>
        </LpLoadGuard>
      </Stack>
    </Stack>
  );
}

export default function AdminPanelCreateRole() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { single: role, create },
    actions: { createRole, resetRole }
  } = useAdminPanelRolesApi();
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb, } = useLpBreadcrumbs();

  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.createRole"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/roles",
        t("lp-ui-admin-panel:s.headersCrumbs.roles")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.createRole"))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);
  useMountEffect(() => {resetRole();});
  useEffect(() => {
    if (role?.id) {
      const roleId = role.id;
      navigateTo("admin-panel/roles/details", { roleId });
    }
  }, [navigateTo, role]);

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      { canManage
      ? (<CreateRoleForm create={create} createRole={createRole} t={t}/>)
      : (<Typography variant="body1" color="error">{t("lp-ui-admin-panel:s.common.accessDenied")}</Typography>)
      }
    </Stack>
  );
}
