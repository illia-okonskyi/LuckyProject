import Apis from "../../components/Content/Apis/Apis";

import AnalyticsRoundedIcon from "@mui/icons-material/AnalyticsRounded";

export function navItemsApisApiMetadataProvider(id) {
  if (id === "apis") {
    return {
      toBuilder: () => "/apis",
      component: Apis,
      navIcon: AnalyticsRoundedIcon
    };
  }

  return null;
}

export function navItemsApisApiMatcher(api) {
  if (api.name === "API" && api.features.includes("Manage")) {
    return {
      id: "apis",
      navIndex: 1,
      navHeaderI18nKey: "lp-ui-shell:s.nav.apis",
      route: "/apis",
      childs: []
    };
  }

  return null;
}

