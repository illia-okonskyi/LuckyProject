import { useSearchParams } from "react-router-dom";
import { useAppAuthContext } from "../../../../hooks/contexts";
import { useTimeout } from "usehooks-ts";

export default function AuthLogout() {
  const { localLogoutCallback } = useAppAuthContext();
  const [searchParams] = useSearchParams();

  const error = searchParams.get("lp_err");
  const result = { success: error === null, error };
  useTimeout(() => localLogoutCallback(result), 0);
  return null;
}
