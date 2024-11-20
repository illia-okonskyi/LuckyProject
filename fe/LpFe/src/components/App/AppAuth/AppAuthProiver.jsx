import { createContext, useCallback, useEffect, useState } from "react";
import { useAuth } from "react-oidc-context";
import { useTranslation } from "react-i18next";

import { oidcConfig } from "./config";
import axios from "axios";

import { useAppAuthStore } from "../../../features/useAppAuthStore";
import { useLpNavigation } from "../../../features/navigation";
import { useLpNotifications } from "../../../features/notifications";

//#region Internals
function useGetUserRequest({ queryProvider, handler }) {
  const serverRequestAsync = useCallback(
    async function (ctx, query, handler) {
      try {
        const response = await query;
        handleResult(handler, { ctx, status: "success", user: response.data.lpApiPayload });
      }
      catch (e) {
        handleResult(handler, { ctx, status: "error", error: e });
      }
    },
    []);

  function handleResult(handler, result) {
    try {
      handler(result);
    } catch (e) {
      console.error("useGetProfileRequest.handler.error", e);
    }
  }

  function serverRequest(ctx = null) {
    const f = (ctx, q) => serverRequestAsync(ctx, q, handler);

    const query = queryProvider(ctx);
    if (query !== null) {
      f(ctx, query);
    }
  }

  return useCallback(serverRequest, [queryProvider, handler, serverRequestAsync]);
}

function getAuthUserAccessToken(user) {
  if (!user?.access_token) {
    return null;
  }
  return `Bearer ${user.access_token}`;
}
//#endregion

//#region AppAuthContext
const AppAuthContext = createContext({
  getAppUser: null,
  getAcccessToken: null,
  getApiEndpoint: null,

  signIn: null,
  logout: null,
  forceRenewAppAuth: null,
  notificationsSubscribe: null,

  localSignInCallback: null,
  localLogoutCallback: null,
  authError: null
});
//#endregion

//#region AppAuthProvider
export default function AppAuthProvider({ appComponent }) {
  const auth = useAuth();
  const { i18n } = useTranslation(["ui", "lp-authserver-errors", "lp-validation"]);
  const {
    getAppUser,
    getAcccessToken,
    getApiEndpoint,
    setAppAuth,
    resetAppAuth
  } = useAppAuthStore();
  const {
    onStartSignIn: navOnStartSignIn,
    onAuthorizeResult: navOnAuthorizeResult,
    navigateTo,
  } = useLpNavigation();

  const getCurrentSignInState =
    () => JSON.parse(sessionStorage.getItem("currentSignInState") || "false");
  const setCurrentSignInState =
    (value) => sessionStorage.setItem("currentSignInState", JSON.stringify(value));
  const getCurrentLogoutState =
    () => JSON.parse(sessionStorage.getItem("currentLogoutState") || "false");
  const setCurrentLogoutState =
    (value) => sessionStorage.setItem("currentLogoutState", JSON.stringify(value));

  const [authError, setAuthError] = useState(null);

  const signIn = useCallback(() => {
    if (getCurrentSignInState()) {
      return false;
    }

    setCurrentSignInState(true);
    const sessionId = getAppUser()?.sessionId;
    resetAppAuth();
    navOnStartSignIn();
    auth.signinRedirect({ extraQueryParams: { ["lp_sid"]: sessionId } });
    return true;
  }, [getAppUser, resetAppAuth, navOnStartSignIn, auth]);


  const logout = useCallback(() => {
    if (getCurrentLogoutState()) {
      return false;
    }

    setCurrentLogoutState(true);
    const user = getAppUser();
    auth.signoutRedirect({
      extraQueryParams: {
        ["lp_cid"]: oidcConfig.client_id,
        ["lp_ler"]: oidcConfig.post_logout_redirect_uri,
        ["lp_uid"]: user.id,
        ["lp_sid"]: user.sessionId
      }
    });
    return true;
  }, [auth, getAppUser]);


  const localSignInCallback = useCallback(
    (result) => {
      setCurrentSignInState(false);

      if (!result.success) {
        setAuthError({ action: "Sign In", error: result.errorDescription });
        navigateTo("auth-error");
      }
    },
    [setAuthError, navigateTo]
  );


  const localLogoutCallback = useCallback(
    (result) => {
      if (!getCurrentLogoutState()) {
        return;
      }

      setCurrentLogoutState(false);
      resetAppAuth();

      if (result.success) {
        navigateTo("home");
      } else {
        setAuthError({ action: "Logout", error: result.error });
        navigateTo("auth-error");
      }
    },
    [resetAppAuth, navigateTo, setAuthError]
  );

  const {
    hubConnected,
    authorizeAsync: notificationsAuthorizeAsync,
    subscribe: notificationsSubscribe
  } = useLpNotifications({
    forceRenewAppAuth: () => forceRenewAppAuth(),
    onAuthorizeResult: useCallback((r) => {
      const fromSignIn = getCurrentSignInState();
      if (r) {
        // NOTE: there are some unwanted caching for state related to redirects, so,
        //       get data directly from localStorage
        navOnAuthorizeResult(getAppUser(), fromSignIn);
        if (fromSignIn) {
          localSignInCallback({ success: true, error: null, error_description: null });
        }
      } else {
        navOnAuthorizeResult(null, fromSignIn);
        if (fromSignIn) {
          localSignInCallback({
            success: false,
            error: "notifications_authorize_error",
            error_description: "lp-authserver-errors:s.lp.authserver.fe.errors.internal"
          });
        }
      }
    }, [getAppUser, localSignInCallback, navOnAuthorizeResult])
  });

  const getUser = useGetUserRequest({
    queryProvider: (ctx) => axios.get(
      `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/profile`,
      { headers: { "Authorization": ctx.accessToken } }),
    handler: (r) => {
      if (r.status === "success" && r.user !== null) {
        setAppAuth(r.user, r.ctx.accessToken);
        i18n.changeLanguage(r.user.preferredLocale);
        notificationsAuthorizeAsync(r.user.sessionId);
        return;
      }

      resetAppAuth();
      i18n.changeLanguage("en-US");
      if (getCurrentSignInState()) {
        localSignInCallback({
          success: false,
          error: "get_user_error",
          error_description: "lp-authserver-errors:s.lp.authserver.fe.errors.internal"
        });
      }
    },
    deps: [
      setAppAuth,
      resetAppAuth,
      localSignInCallback,
      i18n,
      notificationsAuthorizeAsync]
  });

  const forceRenewAppAuth = useCallback(
    () => {
      // NOTE: there are some unwanted caching for state, so, get data directly from localStorage
      const accessToken = JSON.parse(localStorage.getItem("appAuth")).accessToken ||
        getAuthUserAccessToken(auth.user);
      if (!accessToken) {
        return;
      }

      getUser({ accessToken });
    },
    [auth.user, getUser]);

  useEffect(
    () => { return auth.events.addSilentRenewError(() => resetAppAuth()); },
    [auth.events, resetAppAuth]);
  useEffect(
    () => {
      return auth.events.addUserLoaded(
        (u) => { getUser({ accessToken: getAuthUserAccessToken(u) }); });
    },
    [auth, auth.events, getUser]);
  useEffect(() => {
    if (hubConnected && getAcccessToken()) {
      forceRenewAppAuth();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hubConnected]);

  return (
    <AppAuthContext.Provider value={{
      getAppUser,
      getAcccessToken,
      getApiEndpoint,

      signIn,
      logout,
      forceRenewAppAuth,
      notificationsSubscribe,

      localSignInCallback,
      localLogoutCallback,

      authError,
    }}>
      {appComponent}
    </AppAuthContext.Provider>
  );
}
//#endregion

export { AppAuthContext };
