//#region Imports
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { useForm, Controller } from "react-hook-form";

import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import TextField from "@mui/material/TextField";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";

import { LpValidationFieldError, LpValidationSummary } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelUsersApi } from "../../../../../features/apis/adminPanel/useAdminPanelUsersApi";
import { useAuthProfile } from "../../../../../features/auth/useAuthProfile";
//#endregion

//#region UserForm
function UserForm({
  user,
  isWeb,
  locales,
  canManage,
  update,
  deleteState,
  updateUser,
  deleteUser,
  navigateTo,
  t}) {
  const { handleSubmit, control } = useForm({
    values: {
      userName: user?.userName || "",
      email: user?.email || "",
      phoneNumber: user?.phoneNumber || "",
      fullName: user?.fullName || "",
      telegramUserName: user?.telegramUserName || "",
      preferredLocale: user?.preferredLocale || "en-US",
    },
  });

  const canEdit = canManage && (!(user?.isSealed || false));

  const onSubmit = (data) => {
    const request = isWeb
    ? {
      id: user.id,
      email: data.email,
      phoneNumber: data.phoneNumber,
      fullName: data.fullName,
      telegramUserName: data.telegramUserName,
      preferredLocale: data.preferredLocale
    } : {
      id: user.id,
      email: data.email,
      phoneNumber: data.phoneNumber,
      preferredLocale: data.preferredLocale
    };

    updateUser({ isWeb, request });
  };

  const onDelete = () => {
    deleteUser({ id: user.id});
    navigateTo("admin-panel/users");
  };

  const onRoles = () => {
    navigateTo("admin-panel/users/roles", { userId: user.id });
  };  

  const onPermissions = () => {
    navigateTo("admin-panel/users/permissions", { userId: user.id });
  };  

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <LpLoadGuard loading={!user}>
        <Controller
          name="userName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:userName")}
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
              label={t("lp-ui-admin-panel:s.common.email")}
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
              label={t("lp-ui-admin-panel:s.common.phoneNumber")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="PhoneNumber" />

        {isWeb && (<>
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
          <LpValidationFieldError validation={update.validation} vkey="FullName" />
          </>)}

        {isWeb && (<>
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
          <LpValidationFieldError validation={update.validation} vkey="TelegramUserName" />
          </>)}

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
        <LpValidationFieldError validation={update.validation} vkey="PreferredLocale" />

        <Stack spacing={1} direction="row">
          {canEdit && (<LpLoadGuard loading={update.pending}>
            <Button variant="contained" onClick={handleSubmit(onSubmit)}>
              {t("lp-ui-admin-panel:s.common.update")}
            </Button>
          </LpLoadGuard>)}
          {canEdit && isWeb && (<LpLoadGuard loading={deleteState.pending}>
            <Button variant="contained" color="error" onClick={onDelete}>
              {t("lp-ui-admin-panel:s.common.delete")}
            </Button>
          </LpLoadGuard>)}
          <Button variant="contained" color="secondary" onClick={onRoles}>
            {t("lp-ui-admin-panel:s.common.roles")}
          </Button>
          <Button variant="contained" color="secondary" onClick={onPermissions}>
            {t("lp-ui-admin-panel:s.common.permissions")}
          </Button>
        </Stack>
      </LpLoadGuard>
    </Stack>
  );
}
//#endregion

//#region ResetUserPasswordForm
const ResetUserPasswordForm = ({
  user,
  resetPassword,
  resetWebUserPassword,
  t
}) => {
  const { handleSubmit, control, reset } = useForm({
    values: {
      newPassword: "",
      newPasswordRepeat: ""
    },
  });

  const onSubmit = (data) => {
    resetWebUserPassword({
      request: {
        id: user.id,
        newPassword: data.newPassword,
        newPasswordRepeat: data.newPasswordRepeat
      }
    });
    reset();
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
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
        name="newPassword"
        control={control}
        render={({ field: { onChange, onBlur, value, ref } }) => (
          <TextField
            fullWidth
            label={t("lp-ui-admin-panel:s.common.newPassword")}
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
            label={t("lp-ui-admin-panel:s.common.newPasswordRepeat")}
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
        validation={resetPassword.validation}
        fieldNamesMap={{ NewPassword: t("lp-ui-admin-panel:s.common.newPassword") }} />

      <LpLoadGuard loading={resetPassword.pending}>
        <Button
          variant="contained"
          color="secondary"
          onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-admin-panel:s.other.resetPassword")}
        </Button>
      </LpLoadGuard>
    </Stack>
  );
};
//#endregion

//#region AdminPanelUserDetails
export default function AdminPanelUserDetails() {
  const { hasApiFeature } = useAppAuthStore();
  const { userId } = useParams();
  const {
    state: { single: user, update, deleteState, resetPassword },
    actions: { getUser, updateUser, deleteUser, resetWebUserPassword }
  } = useAdminPanelUsersApi();
  const {
    state: { locales },
    actions: { getLocales, }
  } = useAuthProfile();  

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
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageUser", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/users",
        t("lp-ui-admin-panel:s.headersCrumbs.users")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageUser", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, userId]);
  useEffect(() => {
    setCrumbData("userName", user?.userName);
  }, [user, setCrumbData]);

  useMountEffect(() => {
    getLocales();
    getUser({ id: userId });
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  const isWeb = !(user?.isMachineUser || false);

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      <UserForm
        user={user}
        isWeb={isWeb}
        locales={locales}
        canManage={canManage}
        update={update}
        deleteState={deleteState}
        updateUser={updateUser}
        deleteUser={deleteUser}
        navigateTo={navigateTo}
        t={t}/>
      {canManage && isWeb && (<>
        <Divider />
        <ResetUserPasswordForm
          user={user}
          resetPassword={resetPassword}
          resetWebUserPassword={resetWebUserPassword}
          t={t}/>
      </>)}
    </Stack>
  );
}
//#endregion