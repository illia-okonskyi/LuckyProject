import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import HttpApi from "i18next-http-backend";

// eslint-disable-next-line import/no-named-as-default-member
i18n
  .use(HttpApi)
  .use(initReactI18next)
  .init({
    fallbackLng: "en-US",
    load: "currentOnly",
    ns: ["ui", "lp-authserver-errors"],
    defaultNS: "ui",
    debug: true,
    interpolation: {
      escapeValue: false,
    },
    backend: {
      loadPath: "/api/localization/{{lng}}/{{ns}}",
      crossDomain: false,
    }
  });

export default i18n;