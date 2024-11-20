import { SimpleTreeView } from "@mui/x-tree-view/SimpleTreeView";
import AppMenuMainContentBlockItem from "./AppMenuMainContentBlockItem";

import { useTranslation } from "react-i18next";

import { useLpNavigation } from "../../../../features/navigation";

function buildTreeViewItems(items, navItemNavIconProvider, t) {
  if (items.length === 0) {
    return null;
  }

  return items
    .filter((item) => item.navHeaderI18nKey !== null)
    .sort((a, b) => a.navIndex - b.navIndex)
    .map((item) => (
      <AppMenuMainContentBlockItem
        key={item.id}
        itemId={item.id}
        label={t(item.navHeaderI18nKey)}
        labelIcon={navItemNavIconProvider(item.id)}>
          {buildTreeViewItems(item.childs, navItemNavIconProvider, t)}
      </AppMenuMainContentBlockItem>
  ));
}

export default function AppMenuMainContentBlock({ closeMobile }) {
  const { useNavItems, navItemNavIconProvider, navigateTo } = useLpNavigation();
  const { t } = useTranslation(["lp-ui-shell"]);
  const navItems = useNavItems();

  const handleClick = (_e, id) => {
    if (!navigateTo(id)) {
      return;
    }

    if (closeMobile) {
      closeMobile();
    }
  };

  return (
    <SimpleTreeView
      sx={{ textAlign: "left", p:1 }}
      disableSelection
      onItemClick={handleClick}>
      {buildTreeViewItems(navItems, navItemNavIconProvider, t)}
    </SimpleTreeView>
  );
}
