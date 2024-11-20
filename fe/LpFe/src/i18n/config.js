import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import HttpApi from "i18next-http-backend";

import { useAppAuthStore } from "../features/useAppAuthStore";

// eslint-disable-next-line import/no-named-as-default-member
i18n
  .use(HttpApi)
  .use(initReactI18next)
  .init({
    fallbackLng: "en-US",
    load: "currentOnly",
    ns: ["lp-authserver-errors", "lp-validation", "lp-ui-shell"],
    defaultNS: "lp-ui-shell",
    debug: true,
    interpolation: {
      escapeValue: false,
    },
    backend: {
      loadPath: (lngs, namespaces) => {
        const ns = namespaces[0];
        if (ns === "lp-authserver-errors") {
          return `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/localization/{{lng}}/lp-authserver-errors`;
        }
        if (ns === "lp-validation") {
          return `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/localization/{{lng}}/lp-validation`;
        }
        if (ns === "lp-ui-shell") {
          return `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/localization/{{lng}}/lp-ui-shell`;
        }

        // NOTE: This is not a react hook but have the use* name
        // eslint-disable-next-line react-hooks/rules-of-hooks
        const { getApiEndpoint } = useAppAuthStore();
        if (ns === "lp-ui-admin-panel"){
          return `${getApiEndpoint("Admin-Panel")}/localization/{{lng}}/lp-ui-admin-panel`;
        }

        return null;
      },
      crossDomain: true,
    }
  });

export default i18n;