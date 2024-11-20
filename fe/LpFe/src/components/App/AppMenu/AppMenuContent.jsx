import Divider from "@mui/material/Divider";
import Stack from "@mui/material/Stack";

import AppMenuAuthContentBlock from "./ContentBlocks/AppMenuAuthContentBlock";
import AppMenuMainContentBlock from "./ContentBlocks/AppMenuMainContentBlock";
import AppMenuShellContentBlock from "./ContentBlocks/AppMenuShellContentBlock";

export default function AppMenuContent({ closeMobile }) {
  return (
    <>
      <AppMenuAuthContentBlock closeMobile={closeMobile} />
      <Divider />
      <Stack sx={{ flexGrow: 1, justifyContent: "space-between" }}>
        <AppMenuMainContentBlock closeMobile={closeMobile}/>
        <AppMenuShellContentBlock closeMobile={closeMobile}/>
      </Stack>
    </>
  );
}
