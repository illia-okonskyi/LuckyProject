import { useState } from "react";

import AppBar from "@mui/material/AppBar";
import Stack from "@mui/material/Stack";
import Toolbar from "@mui/material/Toolbar";
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";

import AppMenuMobile from "../AppMenu/AppMenuMobile";
import AppLogo from "../AppLogo";
import AppBreadcrumbs from "../AppBreadcrumbs";
import AppBreadcrumbsMobile from "../AppBreadcrumbs/AppBreadcrumbsMobile";
import MenuButton from "../../Common/MenuButton";

export default function AppNavbar({ leftMenuOffset }) {
  const [appMenuOpen, setAppMenuOpen] = useState(false);
  const toggleAppMenuDrawer = (newOpen) => () => {
    setAppMenuOpen(newOpen);
  };

  return (
    <AppBar
      position="fixed"
      sx={{
        boxShadow: 0,
        bgcolor: "background.paper",
        backgroundImage: "none",
        borderBottom: "1px solid",
        borderColor: "divider",
        left: { xs: 0, md:leftMenuOffset },
      }}
    >
      <Toolbar variant="regular">
        <Stack
          direction="row"
          sx={{
            justifyContent: "space-between",
            alignItems: "center",
            width: "100%",
          }}
        >

          <Stack direction="row" spacing={1} sx={{ alignItems:"center" }}>
            <AppLogo display={{ xs: "flex", md: "none" }}/>
            <AppBreadcrumbs display={{ xs: "none", md: "flex" }}/>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ justifyContent: "center", alignItems:"center" }}>
            <AppBreadcrumbsMobile sx={{ display: { xs: "auto", md: "none" } }}/>

            <MenuButton
              onClick={toggleAppMenuDrawer(true)}
              sx={{ display: { xs: "auto", md: "none" } }}
            >
              <MenuRoundedIcon />
            </MenuButton>            
          </Stack>

          <AppMenuMobile
            open={appMenuOpen}
            toggleDrawer={toggleAppMenuDrawer}/>
        </Stack>
      </Toolbar>
    </AppBar>
  );
}
