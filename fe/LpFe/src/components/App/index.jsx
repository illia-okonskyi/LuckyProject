import { createTheme, ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import { Provider } from "react-redux";
import { SnackbarProvider, closeSnackbar } from "notistack";

import store from "../../features/store";
import "../../i18n/config";
import { useLocalizeDocumentAttributes } from "../../i18n/useLocalizeDocumentAttributes";

import { APP_LAYOUT } from "./appLayout";
import AppAuth from "./AppAuth";
import AppMenu from "./AppMenu";
import AppNavbar from "./AppNavbar";
import AppMain from "./AppMain";

const defaultTheme = createTheme({ palette: { mode: "dark" } });

export default function App() {
  useLocalizeDocumentAttributes();

  const defaultSnackAction = (snackbarId) => (
    <Button
      size="small"
      startIcon={<CloseRoundedIcon />}
      onClick={() => closeSnackbar(snackbarId)} />
  );

  const appComponent = (
    <Box sx={{ display: "flex" }}>
      <AppNavbar leftMenuOffset={APP_LAYOUT.LEFT_MENU_WIDTH} />
      <AppMenu width={APP_LAYOUT.LEFT_MENU_WIDTH} />
      <AppMain leftMenuOffset={APP_LAYOUT.LEFT_MENU_WIDTH} />
    </Box>
  );

  return (
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline enableColorScheme />
      <Provider store={store}>
        <SnackbarProvider
          autoHideDuration={5000}
          anchorOrigin={{ horizontal: "right", vertical: "top" }}
          variant="info"
          maxSnack={3}
          action={defaultSnackAction}>

          <AppAuth appComponent={appComponent} />

        </SnackbarProvider>
      </Provider>
    </ThemeProvider>
  );
}
