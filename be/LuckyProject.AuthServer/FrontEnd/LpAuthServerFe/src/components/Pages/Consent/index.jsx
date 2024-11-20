import { useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Stack from "@mui/material/Stack";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";

import AuthLayout from "../../Common/AuthLayout";

import { useTitle } from "../../../hooks/useTitle";
import { useAuthDetails } from "../../../hooks/useAuthDetails";

import { buildServerPath } from "../../../services/serverPathBuilder";

function ConsentForm({ userFullName, finishAuthorize }) {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  return (
    <Stack spacing={1} sx={{ justifyContent: "center" }}>
      <Typography variant="subtitle1">
        {t("ui:s.lp.authserver.fe.ui.pages.consent.form.text1")}
        <strong>{` ${userFullName}! `}</strong>
        {t("ui:s.lp.authserver.fe.ui.pages.consent.form.text2")}
      </Typography>
      <Button
        variant="contained"
        color="success"
        fullWidth
        onClick={() => finishAuthorize("yes")}>
        {t("ui:s.lp.authserver.fe.ui.pages.consent.form.accept")}
      </Button>
      <Button
        variant="contained"
        color="error"
        fullWidth
        onClick={() => finishAuthorize("no")}>
        {t("ui:s.lp.authserver.fe.ui.pages.consent.form.reject")}
      </Button>
    </Stack>
  );
}

export default function Consent() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useTitle(t("ui:s.lp.authserver.fe.ui.pages.consent.title"));
  const [searchParams] = useSearchParams();
  const getAuthDetails = useAuthDetails(searchParams);
  const authDetails = getAuthDetails();

  const finishAuthorize = (consentResult) => {
    searchParams.set("lp_cr", consentResult);
    window.location.href = buildServerPath("/api/connect/authorize-finish?") + searchParams;
  };

  const autoAccept = searchParams.get("lp_caa") === "yes";
  if (autoAccept) {
    finishAuthorize("yes");
    return;
  }

  return (
    <>
      <AuthLayout
        request={t("ui:s.lp.authserver.fe.ui.pages.consent.request")}
        clientDisplayName={authDetails.clientDisplayName}>
        <ConsentForm
          userFullName={authDetails.userFullName}
          finishAuthorize={finishAuthorize}
        />
      </AuthLayout>
    </>
  );
}
