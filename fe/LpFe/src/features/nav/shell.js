import Home from "../../components/Content/Shell/Home";
import NotFound from "../../components/Content/Shell/NotFound";
import AuthSignIn from "../../components/Content/Shell/Auth/AuthSignIn";
import AuthLogout from "../../components/Content/Shell/Auth/AuthLogout";
import AuthError from "../../components/Content/Shell/Auth/AuthError";
import Profile from "../../components/Content/Shell/Profile";
import Help from "../../components/Content/Shell/Help";
import About from "../../components/Content/Shell/About";

import HomeRoundedIcon from "@mui/icons-material/HomeRounded";
import HelpRounded from "@mui/icons-material/HelpRounded";
import InfoRounded from "@mui/icons-material/InfoRounded";

export function navItemsShellMetadataProvider(id) {
  if (id === "home") {
    return {
      toBuilder: () => "/",
      component: Home,
      navIcon: HomeRoundedIcon
    };
  }

  if (id === "auth-sign-in") {
    return {
      toBuilder: null,
      component: AuthSignIn,
      navIcon: null
    };
  }

  if (id === "auth-logout") {
    return {
      toBuilder: null,
      component: AuthLogout,
      navIcon: null
    };
  }

  if (id === "auth-error") {
    return {
      toBuilder: () => "/.auth/error",
      component: AuthError,
      navIcon: null
    };
  }

  if (id === "not-found") {
    return {
      toBuilder: null,
      component: NotFound,
      navIcon: null
    };
  }

  if (id === "profile") {
    return {
      toBuilder: () => "/profile",
      component: Profile,
      navIcon: null
    };
  }

  if (id === "help") {
    return {
      toBuilder: () => "/help",
      component: Help,
      navIcon: HelpRounded
    };
  }  

  if (id === "about") {
    return {
      toBuilder: () => "/about",
      component: About,
      navIcon: InfoRounded
    };
  }  


  if (id === "help") {
    return {
      toBuilder: () => "/help",
      component: Help,
      navIcon: null
    };
  }  

  return null;
}

export const homeNavItem = {
  id: "home",
  navIndex: 0,
  navHeaderI18nKey: "lp-ui-shell:s.nav.home",
  route: "/",
  childs: [],
};

export const notFoundNavItem = {
  id: "not-found",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "*",
  childs: [],
};

export const authSignInItem = {
  id: "auth-sign-in",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/.auth/sign-in",
  childs: [],
};

export const authLogoutItem = {
  id: "auth-logout",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/.auth/logout",
  childs: [],
};

export const authErrorItem = {
  id: "auth-error",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/.auth/error",
  childs: [],
};

export const profileNavItem = {
  id: "profile",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/profile",
  childs: [],
};

export const helpNavItem = {
  id: "help",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/help",
  childs: [],
};

export const aboutNavItem = {
  id: "about",
  navIndex: 0,
  navHeaderI18nKey: null,
  route: "/about",
  childs: [],
};

export const defaultNavItems = [
  homeNavItem,
  authSignInItem,
  authLogoutItem,
  authErrorItem,
  helpNavItem,
  aboutNavItem,
  notFoundNavItem
];