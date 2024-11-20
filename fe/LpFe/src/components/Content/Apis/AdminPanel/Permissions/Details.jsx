import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { useForm, useFieldArray, Controller } from "react-hook-form";

import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import FormControlLabel from "@mui/material/FormControlLabel";
import Switch from "@mui/material/Switch";
import IconButton from "@mui/material/IconButton";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import AddCircleRoundedIcon from "@mui/icons-material/AddCircleRounded";
import RemoveCircleRoundedIcon from "@mui/icons-material/RemoveCircleRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelPermissionsApi } from "../../../../../features/apis/adminPanel/useAdminPanelPermissionsApi";

function PermissionForm({
  permission,
  canManage,
  update,
  deleteState,
  updatePermission,
  deletePermission,
  navigateTo,
  t}) {
  const { handleSubmit, control } = useForm({
    values: {
      name: permission?.name || "",
      type: permission?.type || "",
      fullName: permission?.fullName || "",
      description: permission?.description || "",
      allow: permission?.allow || false,
      level: permission?.level || 0,
      passkeys: permission?.passkeys || [],
    },
  });

  const {
    fields: pkFields,
    append: appendPk,
    remove: removePk
  } = useFieldArray({control, name: "passkeys" });

  const canEdit = canManage && (!(permission?.isSealed || false));

  const onSubmit = (data) => {
    updatePermission({ request: {
      id: permission.id,
      description: data.description,
      allow: permission.type === "Binary" ? data.allow : null,
      level: permission.type === "Level" ? data.level : null,
      passkeys: permission.type === "Passkey" ? data.passkeys : null,
    }});
  };

  const onDelete = () => {
    deletePermission({ id: permission.id});
    navigateTo("admin-panel/permissions");
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <LpLoadGuard loading={!permission}>
        <Controller
          name="name"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.name")}
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
          name="type"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.type")}
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
          name="fullName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.fullName")}
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
          name="description"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.description")}
              size="small"
              multiline
              maxRows={4}
              slotProps={{ input: { readOnly: !canEdit } }}
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="Description" />

        {(permission?.type === "Binary") && (
          <>
            <Controller
              name="allow"
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <FormControlLabel
                  label={t("lp-ui-admin-panel:s.common.allow")}
                  control={(<Switch
                    disabled={!canEdit}
                    checked={value}
                    onChange={onChange}
                    onBlur={onBlur}
                    inputRef={ref}/>
                  )}
                />
              )}
            />
            <LpValidationFieldError validation={update.validation} vkey="Allow" />
          </>
          )
        }

        {(permission?.type === "Level") && (
          <>
            <Controller
              name="level"
              control={control}
              render={({ field: { onChange, onBlur, value, ref } }) => (
                <TextField
                  fullWidth
                  label={t("lp-ui-admin-panel:s.common.level")}
                  size="small"
                  type="number"
                  slotProps={{ input: { readOnly: !canEdit } }}
                  value={value}
                  onChange={onChange}
                  onBlur={onBlur}
                  inputRef={ref}
                />
              )}
            />
            <LpValidationFieldError validation={update.validation} vkey="Level" />
          </>
          )
        }        

        {(permission?.type === "Passkey") && (
          <>
            {pkFields.map((f, i) => (
              <Stack key={f.id} spacing={1} width="100%">
                <Stack spacing={1} direction="row" width="100%">
                  <Controller
                    name={`passkeys.${i}`}
                    control={control}
                    render={({ field: { onChange, onBlur, value, ref } }) => (
                      <TextField
                        fullWidth
                        label={t("lp-ui-admin-panel:s.common.passkey")}
                        size="small"
                        slotProps={{ input: { readOnly: !canEdit } }}
                        value={value}
                        onChange={onChange}
                        onBlur={onBlur}
                        inputRef={ref}
                      />)}
                    />
                    <IconButton size="small" onClick={() => removePk(i)}>
                      <RemoveCircleRoundedIcon/>
                    </IconButton>
                  </Stack>
  
                  <LpValidationFieldError validation={update.validation} vkey={`Passkeys[${i}]`} />
                </Stack>)
              )
            }
            <IconButton size="small" onClick={() => appendPk("")}>
              <AddCircleRoundedIcon/>
            </IconButton>
          </>
        )}

        {canEdit && (
          <Stack spacing={1} direction="row">
            <LpLoadGuard loading={update.pending}>
              <Button variant="contained" onClick={handleSubmit(onSubmit)}>
                {t("lp-ui-admin-panel:s.common.update")}
              </Button>
            </LpLoadGuard>
            <LpLoadGuard loading={deleteState.pending}>
              <Button variant="contained" color="error" onClick={onDelete}>
                {t("lp-ui-admin-panel:s.common.delete")}
              </Button>
            </LpLoadGuard>

          </Stack>
        )}
      </LpLoadGuard>
    </Stack>
  );
}

export default function AdminPanelPermissionDetails() {
  const { hasApiFeature } = useAppAuthStore();
  const { permissionId } = useParams();
  const {
    state: { single: permission, update, deleteState },
    actions: { getPermission, updatePermission, deletePermission }
  } = useAdminPanelPermissionsApi();
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
    const displayName = crumbData.permissionName || permissionId;
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.managePermission", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/permissions",
        t("lp-ui-admin-panel:s.headersCrumbs.permissions")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.managePermission", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, permissionId]);
  useEffect(() => {
    setCrumbData("permissionName", permission?.name);
  }, [permission, setCrumbData]);

  useMountEffect(() => {
    getPermission({ id: permissionId });
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      <PermissionForm
        permission={permission}
        canManage={canManage}
        update={update}
        deleteState={deleteState}
        updatePermission={updatePermission}
        deletePermission={deletePermission}
        navigateTo={navigateTo}
        t={t}/>
    </Stack>
  );
}
