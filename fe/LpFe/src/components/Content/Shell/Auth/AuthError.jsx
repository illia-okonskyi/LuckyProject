import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import HomeRoundedIcon from "@mui/icons-material/HomeRounded";

import { useTranslation } from "react-i18next";

import { useAppAuthContext } from "../../../../hooks/contexts";
import { useLpNavigation } from "../../../../features/navigation";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";
import { useMountEffect } from "../../../../hooks/useMountEffect";

export default function AuthError() {
  const { t } = useTranslation(["lp-ui-shell", "lp-authserver-errors"]);
  const { authError: { action, error } } = useAppAuthContext();
  const { navigateTo, setHeader } = useLpNavigation();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  useMountEffect(() => {
    setHeader(t("lp-ui-shell:s.auth.error.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.auth.error.crumb"))]);
  });

  return (
    <Stack spacing={1} alignItems="center">
      <Typography variant="body1">{t(action)}</Typography>
      <Typography variant="body2">{t(error)}</Typography>

      <Button
        variant="contained"
        startIcon={<HomeRoundedIcon />}
        onClick={() => navigateTo("home")}>
        {t("lp-ui-shell:s.nav.home")}
      </Button>
    </Stack>
  );
}
