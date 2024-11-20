import { useEffect } from "react";

import Typography from "@mui/material/Typography";

import { useTranslation } from "react-i18next";

import { useLpLayoutContext } from "../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";

export default function NotFound() {
  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);
  useEffect(() => {
    setHeader(t("lp-ui-shell:s.notFound.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.notFound.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  return (
    <Typography variant="body1">{t("lp-ui-shell:s.notFound.text")}</Typography>
  );
}
