import { useEffect } from "react";

import Typography from "@mui/material/Typography";
import { useLpLayoutContext } from "../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";

import { useTranslation } from "react-i18next";

export default function About() {
  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);

  useEffect(() => {
    setHeader(t("lp-ui-shell:s.about.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.about.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  return (
    <Typography variant="body1">{t("lp-ui-shell:s.about.text")}</Typography>
  );
}
