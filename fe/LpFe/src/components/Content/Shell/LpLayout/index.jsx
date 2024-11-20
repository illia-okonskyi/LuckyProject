import { createContext } from "react";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import Divider from "@mui/material/Divider";

import { useLpNavigation } from "../../../../features/navigation";

const LpLayoutContext = createContext({
  setHeader: null,
});

export default function LpLayout({ children }) {
  const { useHeader, setHeader } = useLpNavigation();
  const header = useHeader();

  return (
    <Paper>
      <Stack padding={1} useFlexGap>
        <Typography variant="h5" color="primary" textAlign="center">{header}</Typography>
        <Divider sx={{ marginBottom: 2, borderBottomWidth: 3 }} />
        <Box display="flex" justifyContent="center" alignItems="center">
          <LpLayoutContext.Provider value={{ setHeader }}>
            {children}
          </LpLayoutContext.Provider>
        </Box>
      </Stack>
    </Paper>
  );
}

export { LpLayoutContext };