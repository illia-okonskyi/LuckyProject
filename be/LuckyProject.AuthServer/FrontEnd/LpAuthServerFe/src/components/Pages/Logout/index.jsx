import { useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

import AuthLayout from "../../Common/AuthLayout/index";

import { useTitle } from "../../../hooks/useTitle";
import { useAuthDetails } from "../../../hooks/useAuthDetails";

import { buildServerPath } from "../../../services/serverPathBuilder";

const ErrorMessage = ({ message }) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  return (
    <>
      <Typography variant="overline" color="error">{t(message)}</Typography>
    </>
  );
};

function LogoutForm({ userFullName, finishLogout }) {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  return (
    <Stack spacing={1}>
      <Typography variant="subtitle1">
        {t("ui:s.lp.authserver.fe.ui.pages.logout.form.text1")}
        <strong>{` ${userFullName}! `}</strong>
        {t("ui:s.lp.authserver.fe.ui.pages.logout.form.text2")}
      </Typography>
      <Button
        variant="contained"
        color="primary"
        sx={{ m: 1 }}
        fullWidth
        onClick={() => finishLogout("app")}>
        {t("ui:s.lp.authserver.fe.ui.pages.logout.form.app")}
      </Button>
      <Button
        variant="contained"
        color="secondary"
        sx={{ m: 1 }}
        fullWidth
        onClick={() => finishLogout("full")}>
        {t("ui:s.lp.authserver.fe.ui.pages.logout.form.full")}
      </Button>
    </Stack>
  );
}

export default function Logout() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useTitle(t("spages.logout.title"));
  const [searchParams] = useSearchParams();
  const getAuthDetails = useAuthDetails(searchParams);
  const authDetails = getAuthDetails();

  const finishLogout = (logoutResult) => {
    searchParams.set("lp_lr", logoutResult);
    window.location.href = buildServerPath("/api/connect/logout-finish?") + searchParams;
  };

  const errorMessage = searchParams.get("lp_err");
  const children = !errorMessage
    ? (<LogoutForm userFullName={authDetails.userFullName} finishLogout={finishLogout} />)
    : (<ErrorMessage message={errorMessage} />);

  return (
    <>
      <AuthLayout
        request={t("ui:s.lp.authserver.fe.ui.pages.logout.request")}
        clientDisplayName={authDetails.clientDisplayName}>
        {children}
      </AuthLayout>
    </>
  );
}
