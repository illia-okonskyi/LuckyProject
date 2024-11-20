export function useAppAuthStore() {
  const getAppAuth = () => {
   const item = localStorage.getItem("appAuth");
   if (!item) {
    return { user: null, accessToken: null };
   }

   return JSON.parse(item);
  };
  const setAppAuth = (user, accessToken) => {
    localStorage.setItem("appAuth", JSON.stringify({ user: user, accessToken: accessToken }));
  };

  const getAppUser = () => getAppAuth().user;
  const getAcccessToken = () => getAppAuth().accessToken;
  const getApiEndpoint = (apiName) => {
    const user = getAppUser();
    if (user === null) {
      return null;
    }
    return user.apis.find((a) => a.name === apiName)?.endpoint || null;
  };
  const resetAppAuth = () => setAppAuth(null, null);

  const hasApiFeature = (apiName, feature) => {
    const user = getAppUser();
    if (user === null) {
      return false;
    }
    return user.apis.find((a) => a.name === apiName)?.features?.includes(feature) || false;
  };

  return { getAppUser, getAcccessToken, getApiEndpoint, setAppAuth, resetAppAuth, hasApiFeature };
}