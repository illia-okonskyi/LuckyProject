import Drawer, { drawerClasses } from "@mui/material/Drawer";
import Toolbar from "@mui/material/Toolbar";
import MenuButton from "../../Common/MenuButton";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import Divider from "@mui/material/Divider";

import AppMenuContent from "./AppMenuContent";

function AppMenuMobile({ open, toggleDrawer }) {
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
      <Toolbar sx={{ display: "flex", justifyContent: "center"}}>
        <MenuButton onClick={toggleDrawer(false)}><CloseRoundedIcon/></MenuButton>
      </Toolbar>
      <Divider/>
      <AppMenuContent closeMobile={toggleDrawer(false)}/>
    </Drawer>
  );
}

export default AppMenuMobile;
