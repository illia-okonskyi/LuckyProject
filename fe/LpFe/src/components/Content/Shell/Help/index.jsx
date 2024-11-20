import { useEffect } from "react";

import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";

import { useTranslation } from "react-i18next";

import { useLpLayoutContext } from "../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";

export default function Help() {
  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);
  useEffect(() => {
    setHeader(t("lp-ui-shell:s.help.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.help.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  const clearStorage = () => {
    localStorage.clear();
    sessionStorage.clear();
    window.location.reload();
  };

  return (
    <Stack spacing={2} padding={1} alignItems="center">
      <Typography variant="body1">
        {t("lp-ui-shell:s.help.text")}
      </Typography>
      <Button variant="contained" onClick={clearStorage}>
        {t("lp-ui-shell:s.help.clearStorage")}
      </Button>
    </Stack>
  );
}
