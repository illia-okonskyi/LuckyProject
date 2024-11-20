import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useForm, Controller } from "react-hook-form";

import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";

import { LpValidationSummary } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelUsersApi } from "../../../../../features/apis/adminPanel/useAdminPanelUsersApi";
import { useAuthProfile } from "../../../../../features/auth/useAuthProfile";
import { useMountEffect } from "../../../../../hooks/useMountEffect";

function CreateUserForm({ create, locales, createUser, t}) {
  const { handleSubmit, control } = useForm({
    values: {
      userName: "",
      email: "",
      phoneNumber: "",
      fullName: "",
      password: "",
      telegramUserName: "",
      preferredLocale: "en-US",
    },
  });

  const onSubmit = (data) => {
    createUser({ request: {
      userName: data.userName,
      email: data.email,
      phoneNumber: data.phoneNumber,
      fullName: data.fullName,
      password: data.password,
      telegramUserName: data.telegramUserName,
      preferredLocale: data.preferredLocale
    }});
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <Controller
        name="userName"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.userName")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
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
      <Controller
        name="fullName"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.fullName")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <Controller
        name="telegramUserName"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.tgUserName")}
            size="small"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
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
      <Stack sx={{ textAlign: "left" }}>
        <Typography variant="body1">
          {t("lp-ui-admin-panel:s.common.passwordRules.title")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-admin-panel:s.common.passwordRules.1")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-admin-panel:s.common.passwordRules.2")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-admin-panel:s.common.passwordRules.3")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-admin-panel:s.common.passwordRules.4")}
        </Typography>
      </Stack>

      <Controller
        name="password"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.password")}
            size="small"
            type="password"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />
      <LpValidationSummary
        validation={create.validation}
        fieldNamesMap={{
          User: t("lp-ui-admin-panel:s.common.userName"),
          UserName: t("lp-ui-admin-panel:s.common.userName"),
          NormalizedUserName: t("lp-ui-admin-panel:s.common.userName"),
          NormalizedEmail: t("lp-ui-admin-panel:s.common.email"),
          PhoneNumber: t("lp-ui-admin-panel:s.common.phoneNumber"),
          FullName: t("lp-ui-admin-panel:s.common.fullName"),
          TelegramUserName: t("lp-ui-admin-panel:s.common.tgUserName"),
          PreferredLocale: t("lp-ui-admin-panel:s.common.preferredLocale"),
          Password: t("lp-ui-admin-panel:s.common.password"),
          }}/>

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

export default function AdminPanelCreateUser() {
  const { hasApiFeature } = useAppAuthStore();
  const {
    state: { single: user, create },
    actions: { createUser, resetUser }
  } = useAdminPanelUsersApi();
  const {
    state: { locales },
    actions: { getLocales, }
  } = useAuthProfile();  

  const { navigateTo } = useLpNavigation();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb, } = useLpBreadcrumbs();

  const { t, i18n } = useTranslation(["lp-ui-admin-panel"]);
  useEffect(() => {
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.createUser"));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/users",
        t("lp-ui-admin-panel:s.headersCrumbs.users")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.createUser"))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);
  useMountEffect(() => { getLocales(); resetUser(); });
  useEffect(() => {
    if (user?.id) {
      const userId = user.id;
      navigateTo("admin-panel/users/details", { userId });
    }
  }, [navigateTo, user]);

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      { canManage
      ? (<CreateUserForm create={create} locales={locales} createUser={createUser} t={t}/>)
      : (<Typography variant="body1" color="error">{t("lp-ui-admin-panel:s.common.accessDenied")}</Typography>)
      }
    </Stack>
  );
}
