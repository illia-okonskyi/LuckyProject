import * as React from "react";
import { styled } from "@mui/material/styles";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { useTreeItem2 } from "@mui/x-tree-view/useTreeItem2";
import {
  TreeItem2Content,
  TreeItem2IconContainer,
  TreeItem2GroupTransition,
  TreeItem2Root,
  TreeItem2Checkbox,
} from "@mui/x-tree-view/TreeItem2";
import { TreeItem2Icon } from "@mui/x-tree-view/TreeItem2Icon";
import { TreeItem2Provider } from "@mui/x-tree-view/TreeItem2Provider";

import ArticleRoundedIcon from "@mui/icons-material/ArticleRounded";

const AppMenuMainContentBlockItemContent = styled(TreeItem2Content)(({ theme }) => ({
  padding: theme.spacing(0.5, 1),
}));

const AppMenuMainContentBlockItem = React.forwardRef(function AppMenuMainContentBlockItem(props, ref) {
  const { id, itemId, label, labelIcon, disabled, children, ...other } = props;
  const LabelIcon = labelIcon || ArticleRoundedIcon;
  const {
    getRootProps,
    getContentProps,
    getIconContainerProps,
    getCheckboxProps,
    getLabelProps,
    getGroupTransitionProps,
    status,
  } = useTreeItem2({ id, itemId, children, label, disabled, rootRef: ref });

  status.focused = false;
  return (
    <TreeItem2Provider itemId={itemId}>
      <TreeItem2Root {...getRootProps(other)}>
        <AppMenuMainContentBlockItemContent {...getContentProps()}>
          <TreeItem2IconContainer {...getIconContainerProps()}>
            <TreeItem2Icon status={status} />
          </TreeItem2IconContainer>
          <TreeItem2Checkbox {...getCheckboxProps()} />
          <Box sx={{
            display: "flex",
            flexGrow: 1,
            alignItems: "center",
          }}>
          <Box component={LabelIcon}
          sx={{
            pr:1,
            width: 36,
            height: 36
          }}/>
            <Typography
              {...getLabelProps({
                variant: "body2",
                sx: { display: "flex", fontWeight: "inherit", flexGrow: 1 },
              })}
            />
          </Box>
        </AppMenuMainContentBlockItemContent>
        {children && <TreeItem2GroupTransition {...getGroupTransitionProps()} />}
      </TreeItem2Root>
    </TreeItem2Provider>
  );
});

export default AppMenuMainContentBlockItem;

