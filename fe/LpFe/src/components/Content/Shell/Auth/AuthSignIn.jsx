import { useTranslation } from "react-i18next";

import { useSearchParams } from "react-router-dom";
import { useTimeout } from "usehooks-ts";
import { useAppAuthContext, useLpLayoutContext } from "../../../../hooks/contexts";
import { useMountEffect } from "../../../../hooks/useMountEffect";

export default function AuthSignIn() {
  const [searchParams] = useSearchParams();
  const { localSignInCallback } = useAppAuthContext();
  const { setHeader } = useLpLayoutContext();
  const { t } = useTranslation(["lp-ui-shell"]);
  useMountEffect(() => setHeader(t("lp-ui-shell:s.auth.signIn.header")));

  const errorCode = searchParams.get("error");
  const errorDescription = searchParams.get("error_description");

  useTimeout(() => {
    if (errorCode !== null) {
      localSignInCallback({ success: false, errorCode, errorDescription });
    }
  }, 0);

  return null;
}
