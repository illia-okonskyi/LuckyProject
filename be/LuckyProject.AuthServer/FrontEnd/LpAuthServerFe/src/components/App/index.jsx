import { useState } from "react";

import { createTheme, ThemeProvider } from "@mui/material/styles";

import "../../i18n/config";
import useLocalizeDocumentAttributes from "../../i18n/useLocalizeDocumentAttributes";
import { useTranslation } from "react-i18next";

import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { Routes, Route, } from "react-router-dom";

import axios from "axios";

import { useServerRequest } from "../../hooks/useServerRequest";
import { useMountEffect } from "../../hooks/useMountEffect";

import { buildServerPath } from "../../services/serverPathBuilder";

import LocaleSwitcher from "../Common/LocaleSwitcher";
import Home from "../Pages/Home";
import NotFound from "../Pages/NotFound";
import Challenge from "../Pages/Challenge";
import Consent from "../Pages/Consent";
import Logout from "../Pages/Logout";
import ForgotPassword from "../Pages/ForgotPassword";

import logo from "./logo.png";

const defaultTheme = createTheme({ palette: { mode: "dark" } });

export default function App({ renderedAt }) {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useLocalizeDocumentAttributes();

  const [version, setVersion] = useState("<loading>");

  const getVersion = useServerRequest({
    queryProvider: () => axios.get(buildServerPath("/api/version")),
    handler: r => setVersion(r.status === "success" ? r.lpApiPayload : "<error>"),
    deps: [setVersion]
  });
  useMountEffect(() => getVersion());

  return (
    <>
      <ThemeProvider theme={defaultTheme}>
        <CssBaseline />
        <Box
          sx={{
            width: "auto",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <header>
            <img src={logo} className="logo" alt="logo" />
            <br />
            <Typography variant="h4">Lucky Project Auth Server</Typography>
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <Typography variant="subtitle1">{t("ui:s.lp.authserver.fe.ui.version")} {version}</Typography>
              <LocaleSwitcher />
            </Box>
            <hr />
          </header>

          <main>
            <Routes>
              <Route index element={<Home />} />
              <Route path="home" element={<Home />} />
              <Route path="challenge" element={<Challenge />} />
              <Route path="consent" element={<Consent />} />
              <Route path="logout" element={<Logout />} />
              <Route path="forgotPassword" element={<ForgotPassword />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </main>

          <footer>
            <hr />
            <Typography variant="subtitle1">{t("ui:s.lp.authserver.fe.ui.renderedAt", { renderedAt })}</Typography>
          </footer>

        </Box>
      </ThemeProvider>
    </>
  );
}
