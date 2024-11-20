import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import Link from "@mui/material/Link";

import { useLpNavigation } from "../../../features/navigation";

export default function AppLogo(props) {
  const { navigateTo } = useLpNavigation();
  return (
    <Box {...props}>
      <Link style={{ textDecoration: "none", cursor: "pointer" }} onClick={() => navigateTo("home")}>
        <Stack direction="row" spacing={2} sx={{ alignItems: "center" }}>
          <img src="/img/logo.png" width={48} height={48} />
          <Typography variant="h5" color="primary">Lucky Project</Typography>
        </Stack>
      </Link>
    </Box>
  );
}
