//#region Imports
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { useForm, Controller } from "react-hook-form";

import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import TextField from "@mui/material/TextField";

import { LpValidationFieldError } from "../../../../Common/LpValidation";
import LpLoadGuard from "../../../../Common/LpLoadGuard";

import RestartAltRoundedIcon from "@mui/icons-material/RestartAltRounded";

import { useLpLayoutContext } from "../../../../../hooks/contexts";
import { useMountEffect } from "../../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../../features/breadcrumbs";
import { useAppAuthStore } from "../../../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../../../features/navigation";

import { useAdminPanelClientsApi } from "../../../../../features/apis/adminPanel/useAdminPanelClientsApi";
//#endregion

//#region ClientForm
function ClientForm({
  client,
  isWeb,
  canManage,
  deleteState,
  deleteClient,
  navigateTo,
  t}) {
  const { control } = useForm({
    values: {
      type: client?.type || "",
      name: client?.name || "",
      clientId: client?.clientId || "",
      displayName: client?.displayName || "",
      webOrigins: client?.web?.origins || [],
      machineOrigins: client?.machine?.origins || [],
    },
  });

  const canEdit = canManage && (!(client?.isSealed || false));

  const onDelete = () => {
    deleteClient({ id: client.id});
    navigateTo("admin-panel/clients");
  };

  const onUserDetails = () => {
    navigateTo("admin-panel/users/details", { userId: client.machine.userId });
  };  

  const onUserPermissions = () => {
    navigateTo("admin-panel/users/permissions", { userId: client.machine.userId });
  };  

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
      <LpLoadGuard loading={!client}>
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
          name="clientId"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.clientId")}
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
          name="displayName"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-admin-panel:s.common.displayName")}
              size="small"
              slotProps={{ input: { readOnly: true } }}
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />        

        <Typography variant="body1">{t("lp-ui-admin-panel:s.common.origins")}</Typography>
        {(isWeb === true) && (
          <Stack spacing={1} alignSelf="left" width="100%" sx={{ textAlign: "left" }}>
            {client?.web?.origins?.map((o, i) => (
              <Stack key={i}>
                <Typography variant="body1" color="secondary">{o.baseUrl}</Typography>
                <Stack padding={1}>
                  <Typography variant="body2" color="info">- {o.redirectUrl}</Typography>
                  <Typography variant="body2" color="info">- {o.postLogoutRedirectUrl}</Typography>
                </Stack>
              </Stack>
            ))}
          </Stack>
        )}
        {(isWeb === false) && (
          <Stack alignSelf="left" width="100%" sx={{ textAlign: "left" }}>
            {client?.machine?.origins?.map((o, i) => (
              <Typography key={i} variant="body1" color="secondary">{o}</Typography>
            ))}
          </Stack>
        )}

        <Stack spacing={1}>
          {canEdit && (<LpLoadGuard loading={deleteState.pending}>
            <Button variant="contained" color="error" onClick={onDelete}>
              {t("lp-ui-admin-panel:s.common.delete")}

            </Button>
          </LpLoadGuard>)}
          {!isWeb && (<>
            <Button variant="contained" color="primary" onClick={onUserDetails}>
              {t("lp-ui-admin-panel:s.other.userDetails")}
            </Button>
            <Button variant="contained" color="primary" onClick={onUserPermissions}>
              {t("lp-ui-admin-panel:s.other.userPermissions")}
            </Button>
          </>)}
        </Stack>
      </LpLoadGuard>
    </Stack>
  );
}
//#endregion

//#region ResetMachineClientSecretForm
const ResetMachineClientSecretForm = ({
  client,
  secret,
  resetMachineSecret,
  getMachineClientSecret,
  resetMachineClientSecret,
  t
}) => {
  const { handleSubmit, control } = useForm({
    values: {
      secret: secret || ""
    },
  });

  const onSubmit = (data) => {
    resetMachineClientSecret({
      request: {
        id: client.id,
        secret: data.secret
      }
    });
  };

  return (
    <Stack spacing={2} justifyContent="center" alignItems="center">
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
      <LpValidationFieldError validation={resetMachineSecret.validation} vkey="Secret" />   

      <LpLoadGuard loading={resetMachineSecret.pending}>
        <Button
          variant="contained"
          color="secondary"
          onClick={handleSubmit(onSubmit)}>
            {t("lp-ui-admin-panel:s.other.resetSecret")}
        </Button>
      </LpLoadGuard>
    </Stack>
  );
};
//#endregion

//#region AdminPanelUserDetails
export default function AdminPanelClientDetails() {
  const { hasApiFeature } = useAppAuthStore();
  const { clientId } = useParams();
  const {
    state: { single: client, deleteState, secret, resetMachineSecret },
    actions: { getClient, deleteClient, getMachineClientSecret, resetMachineClientSecret }
  } = useAdminPanelClientsApi();

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
    const displayName = crumbData.clientName || clientId;
    setHeader(t("lp-ui-admin-panel:s.headersCrumbs.manageClient", { displayName }));
    setCrumbs([
      buildCrumb(0, null, t("lp-ui-admin-panel:s.headersCrumbs.adminPanel")),
      buildCrumb(
        1,
        "/admin-panel/clients",
        t("lp-ui-admin-panel:s.headersCrumbs.clients")),
      buildCrumb(
        2,
        null,
        t("lp-ui-admin-panel:s.headersCrumbs.manageClient", { displayName }))
      ]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t, crumbData, clientId]);
  useEffect(() => {
    setCrumbData("clientName", client?.name);
  }, [client, setCrumbData]);

  useMountEffect(() => {
    getClient({ id: clientId });
  });

  const canManage = hasApiFeature("Admin-Panel", "Manage");
  const isWeb = client !== null ? (!!client.web) : null;

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      <ClientForm
        client={client}
        isWeb={isWeb}
        canManage={canManage}
        deleteState={deleteState}
        deleteClient={deleteClient}
        navigateTo={navigateTo}
        t={t}
      />
      {canManage && !isWeb && (<>
        <Divider />
        <ResetMachineClientSecretForm
          client={client}
          secret={secret}
          getMachineClientSecret={getMachineClientSecret}
          resetMachineSecret={resetMachineSecret}
          resetMachineClientSecret={resetMachineClientSecret}
          t={t}
        />
      </>)}
    </Stack>
  );
}
//#endregion