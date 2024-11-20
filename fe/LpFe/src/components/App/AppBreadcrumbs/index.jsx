import Box from "@mui/material/Box";
import Breadcrumbs from "@mui/material/Breadcrumbs";
import NavigateNextRoundedIcon from "@mui/icons-material/NavigateNextRounded";

import { useNavigate } from "react-router-dom";
import { useLpBreadcrumbs } from "../../../features/breadcrumbs";
import { buildCrumbs } from "./buildCrumbs";

export default function AppBreadcrumbs(props) {
  const { useCrumbs } = useLpBreadcrumbs();
  const navigate = useNavigate();
  const crumbs = useCrumbs();

  return (
    <Box {...props}>
      <Breadcrumbs separator={<NavigateNextRoundedIcon fontSize="small" />}>
        {[...buildCrumbs(crumbs, navigate, null)]}
      </Breadcrumbs>
    </Box>
  );
}
