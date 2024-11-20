import { useEffect } from "react";
import { useTranslation } from "react-i18next";

export function useLocalizeDocumentAttributes() {
  const { i18n } = useTranslation(["ui", "errors"]);

  useEffect(() => {
    if (i18n.resolvedLanguage) {
      document.documentElement.lang = i18n.resolvedLanguage;
      document.documentElement.dir = i18n.dir(i18n.resolvedLanguage);
    }
  }, [i18n, i18n.resolvedLanguage]);
}