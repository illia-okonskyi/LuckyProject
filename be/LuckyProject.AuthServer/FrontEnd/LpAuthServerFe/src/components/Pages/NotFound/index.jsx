import { useTranslation } from "react-i18next";
import { useTitle } from "../../../hooks/useTitle";
import Typography from "@mui/material/Typography";

export default function NotFound() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useTitle(t("ui:s.lp.authserver.fe.ui.pages.notFound.title"));

  return (
    <>
      <Typography sx={{ m: 1 }} variant="h5">{t("ui:s.lp.authserver.fe.ui.pages.notFound.desc")}</Typography> 
    </>
  );
}

