import { useState } from "react";
import { useNavigate } from "react-router-dom";

import Drawer, { drawerClasses } from "@mui/material/Drawer";
import Toolbar from "@mui/material/Toolbar";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import MenuButton from "../../Common/MenuButton";
import Divider from "@mui/material/Divider";
import ArrowDropDownCircleRoundedIcon from "@mui/icons-material/ArrowDropDownCircleRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";

import { useLpBreadcrumbs } from "../../../features/breadcrumbs";
import { buildCrumbs } from "./buildCrumbs";

function AppBreadcrumbsMobileDrawer({ open, toggleDrawer, crumbsElems }) {
  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={toggleDrawer(false)}
      sx={{
        [`& .${drawerClasses.paper}`]: {
          backgroundImage: "none",
          backgroundColor: "background.paper",
        },
      }}
    >
      <Toolbar sx={{ display: "flex", justifyContent: "center" }}>
        <MenuButton onClick={toggleDrawer(false)}><CloseRoundedIcon /></MenuButton>
      </Toolbar>
      <Divider />
      <Stack spacing={1} minWidth={150} padding={1}>
        {[...crumbsElems]}
      </Stack>
    </Drawer>
  );
}

export default function AppBreadcrumbsMobile(props) {
  const [open, setOpen] = useState(false);
  const { useCrumbs } = useLpBreadcrumbs();
  const navigate = useNavigate();
  const crumbs = useCrumbs();

  const toggleOpen = (newOpen) => () => {
    setOpen(newOpen);
  };

  const crumbsElems = buildCrumbs(crumbs, navigate, toggleOpen(false));
  const lastCrumbElem = crumbsElems.length > 0 ? crumbsElems[crumbsElems.length - 1] : null;

  return (
    <Box {...props}>
      <Stack direction="row" spacing={1} sx={{ justifyContent: "center", alignItems: "center" }}>
        {lastCrumbElem}
        <MenuButton
          onClick={toggleOpen(true)}
          sx={{ display: { xs: "auto", md: "none" } }}
        >
          <ArrowDropDownCircleRoundedIcon />
        </MenuButton>
      </Stack>

      <AppBreadcrumbsMobileDrawer open={open} toggleDrawer={toggleOpen} crumbsElems={crumbsElems}/>
    </Box>
  );
}
