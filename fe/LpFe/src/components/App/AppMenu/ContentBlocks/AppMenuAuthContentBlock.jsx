import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import AccountBoxRoundedIcon from "@mui/icons-material/AccountBoxRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";

import { useTranslation } from "react-i18next";

import { useAppAuthContext } from "../../../../hooks/contexts";

import { useLpNavigation } from "../../../../features/navigation";

export default function AppMenuAuthContentBlock({ closeMobile }) {
  const { getAppUser, signIn, logout } = useAppAuthContext();
  const { navigateTo } = useLpNavigation();
  const { t } = useTranslation(["lp-ui-shell"]);

  const user = getAppUser();

  const handleSignIn = () => {
    signIn();
    if (closeMobile) {
      closeMobile();
    }
  };

  const handleProfile = () => {
    navigateTo("profile");
    if (closeMobile) {
      closeMobile();
    }
  };

  const handleLogout = () => {
    logout();
    if (closeMobile) {
      closeMobile();
    }
  };

  if (user === null) {
    return (
      <Stack sx={{ p: 1 }} spacing={1}>
        <Button
          variant="contained"
          fullWidth
          startIcon={<LoginRoundedIcon />}
          onClick={handleSignIn}>
          {t("lp-ui-shell:s.nav.signIn")}
        </Button>
      </Stack>
    );
  }

  return (
    <Stack sx={{ p: 1 }} spacing={1}>
      <Stack direction="row" sx={{ justifyContent: "center", alignItems: "center" }}>
        <Avatar
          sizes="small"
          alt="logo"
          src="/img/logo.png"
          sx={{ width: 24, height: 24 }}
        />
        <Typography component="p" variant="h6" color="primary">
          {user.fullName}
        </Typography>
      </Stack>
      <Button
        variant="outlined"
        color="primary"
        fullWidth
        startIcon={<AccountBoxRoundedIcon />}
        onClick={handleProfile}>
        {t("lp-ui-shell:s.nav.profile")}
      </Button>
      <Button
        variant="outlined"
        color="secondary"
        fullWidth
        startIcon={<LogoutRoundedIcon />}
        onClick={handleLogout}>
        {t("lp-ui-shell:s.nav.logout")}
      </Button>
    </Stack>
  );
}
