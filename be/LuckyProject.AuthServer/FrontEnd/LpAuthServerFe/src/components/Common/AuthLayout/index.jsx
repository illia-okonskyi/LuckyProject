import { useTranslation } from "react-i18next";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

export default function AuthLayout ({ request, clientDisplayName, children }) {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  return (
    <>
      <Box>
        <Typography sx={{ mt: 1, mb: 1 }} variant="h5">
          {`${request} `}
        </Typography>
        <Typography sx={{ mb: 1 }} variant="h5">
          <strong>{` <${clientDisplayName}> `}</strong>
        </Typography>
        <Typography sx={{ mb: 1 }} variant="h5">{t("ui:s.lp.authserver.fe.ui.authLayout.app")}</Typography>
        {children}
      </Box>
    </>
  );
}