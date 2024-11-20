import { useTranslation } from "react-i18next";
import { useTitle } from "../../../hooks/useTitle";

import Typography from "@mui/material/Typography";

export default function Home() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);  
  useTitle(t("ui:s.lp.authserver.fe.ui.pages.home.title"));

  return (
    <>
      <Typography variant="h5" color="info">
        {t("ui:s.lp.authserver.fe.ui.pages.home.desc.title")}
      </Typography>
      <Typography variant="h6" color="info">
        {t("ui:s.lp.authserver.fe.ui.pages.home.desc.text")}
      </Typography>
    </>
  );
}
