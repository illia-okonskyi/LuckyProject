//#region Imports
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import Select from "@mui/material/Select";
import TextField from "@mui/material/TextField";
import FormControl from "@mui/material/FormControl";
import MenuItem from "@mui/material/MenuItem";
import InputLabel from "@mui/material/InputLabel";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";

import { useTranslation } from "react-i18next";

import { LpValidationFieldError, LpValidationSummary } from "../../../Common/LpValidation";
import LpLoadGuard from "../../../Common/LpLoadGuard";

import { useForm, Controller } from "react-hook-form";
import { useMountEffect } from "../../../../hooks/useMountEffect";
import { useLpLayoutContext } from "../../../../hooks/contexts";
import { useAuthProfile } from "../../../../features/auth/useAuthProfile";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";
import { useEffect } from "react";
//#endregion

//#region ProfileForm
const ProfileForm = ({
  profile,
  locales,
  get,
  update,
  getProfile,
  getLocales,
  updateProfile,
  t
}) => {
  const { handleSubmit, control } = useForm({
    values: {
      userName: profile?.userName || "",
      email: profile?.email || "",
      phoneNumber: profile?.phoneNumber || "",
      fullName: profile?.fullName || "",
      telegramUserName: profile?.telegramUserName || "",
      preferredLocale: profile?.preferredLocale || "en-US",
    },
  });

  const onSubmit = (data) => {
    updateProfile({
      request: {
        email: data.email,
        phoneNumber: data.phoneNumber,
        fullName: data.fullName,
        telegramUserName: data.telegramUserName,
        preferredLocale: data.preferredLocale
      }
    });
  };

  useMountEffect(() => { getProfile(); getLocales(); });

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <LpLoadGuard loading={!profile || !locales || get.pending || locales.pending}>
        <Controller
          name="userName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-shell:s.profile.form.userName")}
              size="small"
              slotProps={{ input: { readOnly: true } }}
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
              label={t("lp-ui-shell:s.profile.form.email")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="NormalizedEmail" />

        <Controller
          name="phoneNumber"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-shell:s.profile.form.phoneNumber")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="PhoneNumber" />

        <Controller
          name="fullName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-shell:s.profile.form.fullName")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="FullName" />

        <Controller
          name="telegramUserName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-shell:s.profile.form.tgUserName")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="TelegramUserName" />

        {locales.value &&
          (<Controller
            name="preferredLocale"
            control={control}
            render={({ field: { onChange, onBlur, value, ref } }) => (
              <FormControl fullWidth>
                <InputLabel id="preferredLocale-label">
                  {t("lp-ui-shell:s.profile.form.preferredLocale")}
                </InputLabel>
                <Select
                  labelId="preferredLocale-label"
                  size="small"
                  label={t("lp-ui-shell:s.profile.form.preferredLocale")}
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
        <LpValidationFieldError validation={update.validation} vkey="PreferredLocale" />

        <LpLoadGuard loading={update.pending}>
          <Button variant="contained" onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-shell:s.profile.form.updateProfile")}
          </Button>
        </LpLoadGuard>
      </LpLoadGuard>
    </Stack>
  );
};
//#endregion

//#region UpdatePasswordForm
const UpdatePasswordForm = ({
  updatePassword,
  updateUserPassword,
  t
}) => {
  const { handleSubmit, control, reset } = useForm({
    values: {
      oldPassword: "",
      newPassword: "",
      newPasswordRepeat: ""
    },
  });

  const onSubmit = (data) => {
    updateUserPassword({
      request: {
        oldPassword: data.oldPassword,
        newPassword: data.newPassword,
        newPasswordRepeat: data.newPasswordRepeat
      }
    });
    reset();
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <Controller
        name="oldPassword"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-shell:s.profile.form.oldPassword")}
            size="small"
            type="password"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />

      <Stack sx={{ textAlign: "left" }}>
        <Typography variant="body1">
          {t("lp-ui-shell:s.profile.form.passwordRules.title")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-shell:s.profile.form.form.passwordRules.1")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-shell:s.profile.form.form.passwordRules.2")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-shell:s.profile.form.form.passwordRules.3")}
        </Typography>
        <Typography variant="body2">
          {t("lp-ui-shell:s.profile.form.form.passwordRules.4")}
        </Typography>
      </Stack>

      <Controller
        name="newPassword"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-shell:s.profile.form.newPassword")}
            size="small"
            type="password"
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            inputRef={ref}
          />
        )}
      />

      <Controller
        name="newPasswordRepeat"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-shell:s.profile.form.newPasswordRepeat")}
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
        validation={updatePassword.validation}
        fieldNamesMap={{ NewPassword: t("lp-ui-shell:s.profile.form.newPassword") }} />

      <LpLoadGuard loading={updatePassword.pending}>
        <Button
          variant="contained"
          color="secondary"
          onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-shell:s.profile.form.updatePassword")}
        </Button>
      </LpLoadGuard>
    </Stack>
  );
};
//#endregion

//#region Profile
export default function Profile() {
  const {
    state: { profile, locales, get, update, updatePassword },
    actions: { getProfile, getLocales, updateProfile, updateUserPassword }
  } = useAuthProfile();
  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);
  useEffect(() => {
    setHeader(t("lp-ui-shell:s.profile.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.profile.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  return (
      <Stack spacing={2} width={{xs: "100%", md: "50%"}}>
        <ProfileForm
          profile={profile}
          locales={locales}
          get={get}
          update={update}
          getProfile={getProfile}
          getLocales={getLocales}
          updateProfile={updateProfile}
          t={t}/>
        <Divider />
        <UpdatePasswordForm
          updatePassword={updatePassword}
          updateUserPassword={updateUserPassword}
          t={t}/>
      </Stack>
  );
}
//#endregion