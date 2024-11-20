import { useTheme } from "@mui/material";

import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import ClipLoader from "react-spinners/ClipLoader";

export default function LpLoadSpinner({
  inline = false,
  direction = "column",
  label = null,
  color = "primary",
  size = 35
}) {
  const theme = useTheme();
  const nativeColor = {
    primary: theme.palette.primary.main,
    secondary: theme.palette.secondary.main,
    error: theme.palette.error.main,
    warning: theme.palette.warning.main,
    info: theme.palette.info.main,
    success: theme.palette.success.main
  }[color];

  const display = inline ? "inline-flex" : "flex";
  return (
    <Stack
      spacing={1}
      direction={direction}
      sx={{ display:{display}, alignItems: "center", justifyContent: "center", p: 1 }}>
      <ClipLoader color={nativeColor} size={size} />
      <Typography variant="caption" color={color}>{label}</Typography>
    </Stack>
  );
}
