import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useForm, useFieldArray, Controller } from "react-hook-form";

import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import IconButton from "@mui/material/IconButton";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import AddCircleRoundedIcon from "@mui/icons-material/AddCircleRounded";
import RemoveCircleRoundedIcon from "@mui/icons-material/RemoveCircleRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelClientsApi } from "../../../../../features/apis/adminPanel/useAdminPanelClientsApi";
import { useMountEffect } from "../../../../../hooks/useMountEffect";

function CreateWebClientForm({ createWeb, createWebClient, t}) {
  const { handleSubmit, control } = useForm({
    values: {
      name: "",
      displayName: "",
      origins: []
    },
  });

  const {
    fields: originsFields,
    append: appendOrigin,
    remove: removeOrigin
  } = useFieldArray({control, name: "origins" });

  const onSubmit = (data) => {
    createWebClient({ request: {
      name: data.name,
      displayName: data.displayName,
      origins: data.origins
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
      <LpValidationFieldError validation={createWeb.validation} vkey="Name" />

      <Controller
        name="displayName"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.displayName")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationFieldError validation={createWeb.validation} vkey="DisplayName" />

      <Stack spacing={1} direction="row" alignItems="center">
        <Typography variant="body1">{t("lp-ui-admin-panel:s.common.origins")}</Typography>
        <IconButton size="small" onClick={() => appendOrigin({
          baseUrl: "https://example.com",
          redirect:"/signIn",
          postLogoutRedirect: "/logout"
          })}>
          <AddCircleRoundedIcon/>
        </IconButton>
      </Stack>

      {originsFields.map((f, i) => (
        <Stack key={f.id} spacing={1} width="100%">
          <Stack spacing={1} direction="row" width="100%">
            <Controller
              name={`origins.${i}.baseUrl`}
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <TextField
                  fullWidth
                  label={t("lp-ui-admin-panel:s.common.baseUrl")}
                  size="small"
                  value={value}
                  onChange={onChange}
                  onBlur={onBlur}
                  inputRef={ref}
                />)}
              />
            <Controller
              name={`origins.${i}.redirect`}
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <TextField
                  fullWidth
                  label={t("lp-ui-admin-panel:s.common.redirect")}
                  size="small"
                  value={value}
                  onChange={onChange}
                  onBlur={onBlur}
                  inputRef={ref}
                />)}
              />
            <Controller
              name={`origins.${i}.postLogoutRedirect`}
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <TextField
                  fullWidth
                  label={t("lp-ui-admin-panel:s.common.postLogoutRedirect")}
                  size="small"
                  value={value}
                  onChange={onChange}
                  onBlur={onBlur}
                  inputRef={ref}
                />)}
              />

            <IconButton size="small" onClick={() => removeOrigin(i)}>
              <RemoveCircleRoundedIcon/>
            </IconButton>
          </Stack>

          <LpValidationFieldError validation={createWeb.validation} vkey={`Origins[${i}]`} />
        </Stack>)
      )}

      <Stack spacing={1} direction="row">
        <LpLoadGuard loading={createWeb.pending}>
          <Button variant="contained" onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-admin-panel:s.common.create")}
          </Button>
        </LpLoadGuard>
      </Stack>
    </Stack>
  );
}

export default function AdminPanelCreateWebClient() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { single: client, createWeb },
    actions: { createWebClient, resetClient }
  } = useAdminPanelClientsApi();
  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb, } = useLpBreadcrumbs();

  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.createWebClient"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/clients",
        t("lp-ui-admin-panel:s.headersCrumbs.clients")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.createWebClient"))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);
  useMountEffect(() => { resetClient(); });
  useEffect(() => {
    if (client?.id) {
      const clientId = client.id;
      navigateTo("admin-panel/clients/details", { clientId });
    }
  }, [navigateTo, client]);

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      { canManage
      ? (<CreateWebClientForm createWeb={createWeb} createWebClient={createWebClient} t={t}/>)
      : (<Typography variant="body1" color="error">{t("lp-ui-admin-panel:Access-Denied")}</Typography>)
      }
    </Stack>
  );
}
