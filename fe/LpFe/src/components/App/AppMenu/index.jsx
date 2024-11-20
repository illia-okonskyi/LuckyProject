import Drawer from "@mui/material/Drawer";
import Toolbar from "@mui/material/Toolbar";
import Divider from "@mui/material/Divider";

import AppLogo from "../AppLogo";
import AppMenuContent from "./AppMenuContent";

export default function AppMenu({ width }) {
  return (
    <Drawer
      variant="permanent"
      sx={{
        display: { xs: "none", md: "block" },
        width: {width},
        "& .MuiDrawer-paper": {
            width: width,
            boxSizing: "border-box",
          },
      }}
    >
      <Toolbar>
        <AppLogo display={{ xs: "none", md: "flex" }}/>
      </Toolbar>
      <Divider display={{ xs: "none", md: "flex" }}/>      
      <AppMenuContent />
    </Drawer>
  );
}
