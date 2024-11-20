import { useEffect, useState } from "react";

import Box from "@mui/material/Box";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import FormControl from "@mui/material/FormControl";

import { useTranslation } from "react-i18next";
import { useServerRequest } from "../../../hooks/useServerRequest";
import { useMountEffect } from "../../../hooks/useMountEffect";
import { useLocalStorage } from "../../../hooks/useLocalStorage";

import { buildServerPath } from "../../../services/serverPathBuilder";

import axios from "axios";

const LocaleIcon = () => {
  return (
    <svg
      aria-hidden="true"
      fill="none"
      strokeWidth={2}
      stroke="currentColor"
      viewBox="0 0 24 24"
      width={32}
      height={32}
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="m10.5 21 5.25-11.25L21 21m-9-3h7.5M3 5.621a48.474 48.474 0 0 1 6-.371m0 0c1.12 0 2.233.038 3.334.114M9 5.25V3m3.334 2.364C11.176 10.658 7.69 15.08 3 17.502m9.334-12.138c.896.061 1.785.147 2.666.257m-4.589 8.495a18.023 18.023 0 0 1-3.827-5.802"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};


export default function LocaleSwitcher() {
  const { i18n } = useTranslation(["ui", "lp-authserver-errors"]);
  const [ defaultLocale, setDefaultLocale ] = useLocalStorage("defaultLocale", "en-US");
  const [state, setState] = useState({ status: "loading", locales: null, currentLocale: null });

  const getLocales = useServerRequest({
    queryProvider: () => axios.get(buildServerPath("/api/localization")),
    handler: r => {
      if (r.status === "pending") {
        return;
      }

      if (r.status === "success") {
        const locales = r.lpApiPayload;
        setState({
          status: "ready",
          locales,
          currentLocale: locales.find(l => l.name === defaultLocale) ||
            locales.find(l => l.name === "en-US") || 
            locales[0]
        });
        return;
      }

      setState({ status: "error", locales: null, currentLocale: null });
    },
    deps: [setState]
  });
  useEffect(() => {
    if (state.status === "ready") {
      i18n.changeLanguage(state.currentLocale.name);
      setDefaultLocale(state.currentLocale.name);
    }
  },
  [i18n, state, setDefaultLocale]);
  useMountEffect(() => getLocales());

  let ctrl = null;
  if (state.status === "loading") {
    ctrl = <Select sx={{ m: 1 }} value={1}><MenuItem value={1}>Loading...</MenuItem></Select>;
  } else if (state.status === "error") {
    ctrl = <Select sx={{ m: 1 }} value={1}><MenuItem value={1}>Error</MenuItem></Select>;
  } else {
    ctrl = <Select
      sx={{ m: 1 }}
      value={state.currentLocale}
      label={state.currentLocale.displayName}
      onChange={e => setState({ ...state, currentLocale: e.target.value })}>
      {state.locales.map(l => <MenuItem key={l.name} value={l}>{l.displayName}</MenuItem>)}
    </Select>;
  }

  return (
    <Box sx={{ display: "flex", alignItems: "center" }}>
      <LocaleIcon />
      <FormControl variant="standard">
        {ctrl}
      </FormControl>
    </Box >
  );
}