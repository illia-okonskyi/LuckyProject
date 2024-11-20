import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useForm, useFieldArray, Controller } from "react-hook-form";

import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import IconButton from "@mui/material/IconButton";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import AddCircleRoundedIcon from "@mui/icons-material/AddCircleRounded";
import RemoveCircleRoundedIcon from "@mui/icons-material/RemoveCircleRounded";
import RestartAltRoundedIcon from "@mui/icons-material/RestartAltRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelClientsApi } from "../../../../../features/apis/adminPanel/useAdminPanelClientsApi";
import { useAuthProfile } from "../../../../../features/auth/useAuthProfile";
import { useMountEffect } from "../../../../../hooks/useMountEffect";

function CreateMachineClientForm({
  createMachine,
  secret,
  locales,
  getMachineClientSecret,
  createMachineClient,
  t
}) {
  const { handleSubmit, control } = useForm({
    values: {
      name: "",
      email: "",
      phoneNumber: "",
      preferredLocale: "en-US",
      secret: secret || "",
      origins: []
    },
  });

  const {
    fields: originsFields,
    append: appendOrigin,
    remove: removeOrigin
  } = useFieldArray({control, name: "origins" });

  const onSubmit = (data) => {
    createMachineClient({ request: {
      name: data.name,
      email: data.email,
      phoneNumber: data.phoneNumber,
      preferredLocale: data.preferredLocale,
      secret: data.secret,
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
      <LpValidationFieldError validation={createMachine.validation} vkey="Name" />
      <LpValidationFieldError validation={createMachine.validation} vkey="NormalizedUserName" />

      <Controller
        name="email"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.email")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationFieldError validation={createMachine.validation} vkey="NormalizedEmail" />

      <Controller
        name="phoneNumber"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.phoneNumber")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationFieldError validation={createMachine.validation} vkey="PhoneNumber" />

      <Stack spacing={1} direction="row" width="100%">
        <Controller
          name="secret"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.secret")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <IconButton size="small" onClick={() => getMachineClientSecret()}>
          <RestartAltRoundedIcon />
        </IconButton>
      </Stack>
      <LpValidationFieldError validation={createMachine.validation} vkey="Secret" />      

      {locales.value &&
        (<Controller
          name="preferredLocale"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <FormControl fullWidth>
              <InputLabel id="preferredLocale-label">
                {t("lp-ui-admin-panel:s.common.preferredLocale")}
              </InputLabel>

              <Select 
                size="small"
                labelId="preferredLocale-label"
                label={t("lp-ui-admin-panel:s.common.preferredLocale")}
                value={value}
                onChange={onChange}
                onBlur={onBlur}
                inputRef={ref}
              >
                {locales.value?.map((l) => (<MenuItem key={l.name} value={l.name}>{l.displayName}</MenuItem>))}
              </Select>
            </FormControl>
          )}
        />)
      }
      <LpValidationFieldError validation={createMachine.validation} vkey="PreferredLocale" />      

      <Stack spacing={1} direction="row" alignItems="center">
        <Typography variant="body1">{t("lp-ui-admin-panel:s.common.origins")}</Typography>
        <IconButton size="small" onClick={() => appendOrigin("https://example.com")}>
          <AddCircleRoundedIcon/>
        </IconButton>
      </Stack>

      {originsFields.map((f, i) => (
        <Stack key={f.id} spacing={1} width="100%">
          <Stack spacing={1} direction="row" width="100%">
            <Controller
              name={`origins.${i}`}
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <TextField
                  fullWidth
                  label={t("lp-ui-admin-panel:s.common.origin")}
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

          <LpValidationFieldError validation={createMachine.validation} vkey={`Origins[${i}]`} />
        </Stack>
        )
      )}
      <LpValidationFieldError validation={createMachine.validation} vkey="Origins" />

      <Stack spacing={1} direction="row">
        <LpLoadGuard loading={createMachine.pending}>
          <Button variant="contained" onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-admin-panel:s.common.create")}
          </Button>
        </LpLoadGuard>
      </Stack>
    </Stack>
  );
}

export default function AdminPanelCreateMachineClient() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { single: client, secret, createMachine },
    actions: { createMachineClient, getMachineClientSecret, resetClient }
  } = useAdminPanelClientsApi();
  const {
    state: { locales },
    actions: { getLocales, }
  } = useAuthProfile();  

  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb, } = useLpBreadcrumbs();

  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.createMachineClient"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/clients",
        t("lp-ui-admin-panel:s.headersCrumbs.clients")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.createMachineClient"))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);
  useMountEffect(() => { resetClient(); getLocales(); });
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
      ? (<CreateMachineClientForm
          createMachine={createMachine}
          secret={secret}
          locales={locales}
          createMachineClient={createMachineClient}
          getMachineClientSecret={getMachineClientSecret}
          t={t}
          />)
      : (<Typography variant="body1" color="error">{t("lp-ui-admin-panel:Access-Denied")}</Typography>)
      }
    </Stack>
  );
}
