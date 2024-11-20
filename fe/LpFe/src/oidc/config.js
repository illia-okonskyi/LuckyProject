import { Log as OidcLog , WebStorageStateStore } from "oidc-client-ts";


OidcLog.setLogger(console);

export const oidcConfig = {
  authority: window.CONFIG.AUTH_SERVER_ENDPOINT,
  client_id: window.CONFIG.CLIENT_ID,
  redirect_uri: `${window.CONFIG.SELF_ENDPOINT}/auth/loggedIn`,
  post_logout_redirect_uri: `${window.CONFIG.SELF_ENDPOINT}/auth/loggedOut`,
  scope: "openid profile email phone roles offline_access",
  userStore: new WebStorageStateStore({ store: window.localStorage }),
};