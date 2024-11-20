//#region Imports
import { createSlice } from "@reduxjs/toolkit";
import { useCallback, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate, matchPath, useLocation } from "react-router-dom";

import {
  navItemsShellMetadataProvider,
  homeNavItem,
  authSignInItem,
  authLogoutItem,
  authErrorItem,
  profileNavItem,
  helpNavItem,
  aboutNavItem,
  notFoundNavItem,
  defaultNavItems
} from "./nav/shell";

import { navItemsApisApiMetadataProvider, navItemsApisApiMatcher } from "./nav/apis";
import { navItemsAdminPanelApiMetadataProvider, navItemsAdminPanelApiMatcher } from "./nav/adminPanel";
//#endregion

//#region navItemsMetadataProvider
function navItemsMetadataProvider(id) {
  return navItemsShellMetadataProvider(id) ||
    navItemsApisApiMetadataProvider(id) ||
    navItemsAdminPanelApiMetadataProvider(id) ||
    null;
}

const navItemToBuilder = (id, params) => {
  const builder = navItemsMetadataProvider(id)?.toBuilder;
  if (!builder) {
    return null;
  }

  return builder(params);
};

const navItemComponentProvider = (id) => navItemsMetadataProvider(id)?.component || null;
const navItemNavIconProvider = (id) => navItemsMetadataProvider(id)?.navIcon || null;
//#endregion

//#region navItemsBuilder
const navItemsBuilder = (apis) => {
  const apisNavItems = apis.map((api) => {
    return navItemsApisApiMatcher(api) ||
      navItemsAdminPanelApiMatcher(api) ||
      null;
  }).filter((i) => i !== null);

  return [
    homeNavItem,
    authSignInItem,
    authLogoutItem,
    authErrorItem,
    profileNavItem,
    ...apisNavItems,
    helpNavItem,
    aboutNavItem,
    notFoundNavItem
  ];
};
//#endregion

//#region findNavItem*
const findNavItem = (navItems, pred) => {
  const queue = [...navItems];
  while (queue.length > 0) {
    const item = queue.shift();
    if (pred(item)) {
      return item;
    }

    item.childs.forEach((i) => queue.push(i));
  }

  return null;
};

const findNavItemById = (navItems, id) => {
  if (id === null) {
    return null;
  }

  return findNavItem(navItems, (item) => item.id === id);
};

const findNavItemByPathName = (navItems, pathname) => {
  if (pathname === null) {
    return null;
  }

  return findNavItem(navItems, (item) => {
    if (item.route === null) {
      return false;
    }

    return (matchPath(item.route, pathname) !== null);
  });
};
//#endregion

//#region Slice
export const lpNavigationSlice = createSlice({
  name: "navigation",
  initialState: {
    apisJson: null,
    navItems: defaultNavItems,
    currentNavItem: homeNavItem,
    header: null
  },
  reducers: {
    onAuthorizeResult: (state, action) => {
      const location = action.payload.location;
      const user = action.payload.user;
      const signInPrevPath = action.payload.signInPrevPath;
      const prevPath = signInPrevPath || location.pathname;

      if (user === null) {
        state.apisJson = null;
        state.navItems = defaultNavItems;
        state.currentNavItem = findNavItemByPathName(defaultNavItems, prevPath);
        return;
      }

      const apisJson = JSON.stringify(
        user.apis.map((a) => ({ name: a.name, features: a.features }))
      );
      if (apisJson === state.apisJson) {
        return;
      }

      state.apisJson = apisJson;
      state.navItems = navItemsBuilder(user.apis);
      state.currentNavItem = findNavItemByPathName(state.navItems, prevPath);
    },
    onLocationChanged: (state, action) => {
      const location = action.payload;
      state.currentNavItem = findNavItemByPathName(state.navItems, location.pathname);
    },
    setHeader: (state, action) => {
      state.header = action.payload;
    }
  }
});
export const lpNavigationReducer = lpNavigationSlice.reducer;
//#endregion

//#region useLpNavigation
export function useLpNavigation() {
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const actions = lpNavigationSlice.actions;

  const navigateTo = useCallback(
    function (id, params = null) {
      const to = navItemToBuilder(id, params);
      if (to === null) {
        return false;
      }

      navigate(to);
      return true;
    },
    [navigate]
  );

  const useNavItems = () => useSelector((s) => s.navigation.navItems);
  const useNavItem = (id) => useSelector((s) => findNavItemById(s.navigation.navItems, id));
  const useCurrentNavItem = () => useSelector((s) => s.navigation.currentNavItem);

  const onStartSignIn = () => {
    sessionStorage.setItem("signInPrevPath", location.pathname + location.search + location.hash);
  };

  const onAuthorizeResult = useCallback(
    (user, fromSignIn) => {
      let signInPrevPath = null;
      if (fromSignIn) {
        signInPrevPath = sessionStorage.getItem("signInPrevPath");
        sessionStorage.removeItem("signInPrevPath");
      }

      dispatch(actions.onAuthorizeResult({ user, location, signInPrevPath }));
      if (fromSignIn) {
        navigate(signInPrevPath);
      }
    },
    [actions, dispatch, location, navigate]
  );

  const setHeader = useCallback(
    (h) => {
      document.title = ["Lucky Project", h].filter((t) => !!t).join(" | ");
      dispatch(actions.setHeader(h));
    },
    [actions, dispatch]
  );
  const useHeader = () => useSelector((s) => s.navigation.header);

  useEffect(() => {
    const pathname = location.pathname;
    if (pathname === authSignInItem.route || pathname === authLogoutItem.route) {
      return;
    }

    dispatch(actions.onLocationChanged(location));
  }, [actions, dispatch, location]);

  return {
    onStartSignIn,
    onAuthorizeResult,

    navigateTo,

    useNavItems,
    useNavItem,
    useCurrentNavItem,

    navItemComponentProvider,
    navItemNavIconProvider,

    setHeader,
    useHeader
  };
}
//#endregion