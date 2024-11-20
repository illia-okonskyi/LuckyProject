import { AuthProvider } from "react-oidc-context";
import { oidcConfig } from "./config";
import AppAuthProvider from "./AppAuthProiver";

export default function AppAuth({ appComponent }) {
  return (
    <AuthProvider {...oidcConfig}>
      <AppAuthProvider appComponent={appComponent}/>
    </AuthProvider>
  );
}
