import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { useForm, Controller } from "react-hook-form";

import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelRolesApi } from "../../../../../features/apis/adminPanel/useAdminPanelRolesApi";

function RoleForm({
  role,
  canManage,
  update,
  deleteState,
  updateRole,
  deleteRole,
  navigateTo,
  t}) {
  const { handleSubmit, control } = useForm({
    values: {
      name: role?.name || "",
      description: role?.description || "",
    },
  });

  const canEdit = canManage && (!(role?.isSealed || false));

  const onSubmit = (data) => {
    updateRole({ request: {
      id: role.id,
      name: data.name,
      description: data.description
    }});
  };

  const onDelete = () => {
    deleteRole({ id: role.id});
    navigateTo("admin-panel/roles");
  };

  const onUsers = () => {
    navigateTo("admin-panel/roles/users", { roleId: role.id });
  };  

  const onPermissions = () => {
    navigateTo("admin-panel/roles/permissions", { roleId: role.id });
  };  

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <LpLoadGuard loading={!role}>
        <Controller
          name="name"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.name")}
              size="small"
              slotProps={{ input: { readOnly: !canEdit } }}
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />
        <LpValidationFieldError validation={update.validation} vkey="NormalizedName" />

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

        <Stack spacing={1} direction="row">
          {canEdit && (<LpLoadGuard loading={update.pending}>
            <Button variant="contained" onClick={handleSubmit(onSubmit)}>
              {t("lp-ui-admin-panel:s.common.update")}
            </Button>
          </LpLoadGuard>)}
          {canEdit && (<LpLoadGuard loading={deleteState.pending}>
            <Button variant="contained" color="error" onClick={onDelete}>
              {t("lp-ui-admin-panel:s.common.delete")}
            </Button>
          </LpLoadGuard>)}
          <Button variant="contained" color="secondary" onClick={onUsers}>
            {t("lp-ui-admin-panel:s.common.users")}
          </Button>
          <Button variant="contained" color="secondary" onClick={onPermissions}>
            {t("lp-ui-admin-panel:s.common.permissions")}
          </Button>
        </Stack>
      </LpLoadGuard>
    </Stack>
  );
}

export default function AdminPanelRoleDetails() {
  const { hasApiFeature } = useAppAuthStore();
  const { roleId } = useParams();
  const {
    state: { single: role, update, deleteState },
    actions: { getRole, updateRole, deleteRole }
  } = useAdminPanelRolesApi();
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
    const displayName = crumbData.roleName || roleId;
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageRole", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/roles",
        t("lp-ui-admin-panel:s.headersCrumbs.roles")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageRole", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, roleId]);
  useEffect(() => {
    setCrumbData("roleName", role?.name);
  }, [role, setCrumbData]);

  useMountEffect(() => {
    getRole({ id: roleId });
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      <RoleForm
        role={role}
        canManage={canManage}
        update={update}
        deleteState={deleteState}
        updateRole={updateRole}
        deleteRole={deleteRole}
        navigateTo={navigateTo}
        t={t}/>
    </Stack>
  );
}
