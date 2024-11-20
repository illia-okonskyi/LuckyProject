import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";

import { useTranslation } from "react-i18next";

import { useLpNavigation } from "../../../../features/navigation";
import { createElement } from "react";

export default function AppMenuShellContentBlock({ closeMobile }) {
  const { navItemNavIconProvider, navigateTo } = useLpNavigation();
  const { t } = useTranslation(["lp-ui-shell"]);

  const handleHelpClick = () => {
    navigateTo("help");
    if (closeMobile) {
      closeMobile();
    }
  };

  const handleAboutClick = () => {
    navigateTo("about");
    if (closeMobile) {
      closeMobile();
    }
  };

  return (
    <Stack>
      <Divider />
      <List sx={{ p:1 }} dense>
        <ListItem key="help" disablePadding sx={{ display: "block" }}>
          <ListItemButton onClick={handleHelpClick}>
            <ListItemIcon>{createElement(navItemNavIconProvider("help"))}</ListItemIcon>
            <ListItemText primary={t("lp-ui-shell:s.nav.help")} />
          </ListItemButton>
        </ListItem>
        <ListItem key="about" disablePadding sx={{ display: "block" }}>
          <ListItemButton onClick={handleAboutClick}>
            <ListItemIcon>{createElement(navItemNavIconProvider("about"))}</ListItemIcon>
            <ListItemText primary={t("lp-ui-shell:s.nav.about")} />
          </ListItemButton>
        </ListItem>
      </List>
    </Stack>
  );
}
