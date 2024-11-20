import { useContext } from "react";
import { AppAuthContext } from "../components/App/AppAuth/AppAuthProiver";
import { LpLayoutContext } from "../components/Content/Shell/LpLayout";

export const useAppAuthContext = () => {
  return useContext(AppAuthContext);
};

export const useLpLayoutContext = () => {
  return useContext(LpLayoutContext);
};
