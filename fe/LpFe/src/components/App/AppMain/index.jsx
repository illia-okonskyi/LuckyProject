import { Routes, Route } from "react-router-dom";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";

import LpLayout from "../../Content/Shell/LpLayout";

import { useLpNavigation } from "../../../features/navigation";

function buildNavRoutes(items, navItemComponentProvider) {
  let result = [];
  let queue = [...items];
  while (queue.length > 0) {
    const item = queue.shift();
    result.push((<Route
      key={item.id}
      path={item.route}
      Component={navItemComponentProvider(item.id)}
    />)
    );

    item.childs.forEach((c) => queue.push(c));
  }

  return result;
}

export default function AppMain({ leftMenuOffset }) {
  const { useNavItems, navItemComponentProvider } = useLpNavigation();
  const navItems = useNavItems();

  return (
    <Box sx={{ left: { xs: "0px", md: leftMenuOffset }, width: "100%" }}>
      <Toolbar />
      <main>
        <LpLayout>
          <Routes>
            {[...buildNavRoutes(navItems, navItemComponentProvider)]}
          </Routes>
        </LpLayout>
      </main>
    </Box>
  );
}
