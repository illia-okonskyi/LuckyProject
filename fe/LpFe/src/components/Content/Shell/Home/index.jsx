/* eslint-disable no-unused-vars */
import { useEffect, useState } from "react";

import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";

import { useTranslation } from "react-i18next";

import { useLpLayoutContext, useAppAuthContext } from "../../../../hooks/contexts";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";

import { useLpApiRequest } from "../../../../features/useLpApiRequest";

import { textColumn } from "../../../Common/LpDataList/columns";

import axios from "axios";
import { useMountEffect } from "../../../../hooks/useMountEffect";

export default function Home() {
  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);
  useEffect(() => {
    setHeader(t("lp-ui-shell:s.home.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.home.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  const [values, setValues] = useState({});

  const { getApiEndpoint } = useAppAuthContext();
  const baseEndpoint = `${window.CONFIG.AUTH_SERVER_ENDPOINT}/api/test/test`;

  const getValues = useLpApiRequest({
    queryProvider: (ahp, ctx) => axios.post(
      baseEndpoint,
      ctx.request,
      { headers: { ...ahp() } }),
    onSuccess: (r) => setValues(r.payload.payload.values),
  });


  useMountEffect(() => {
    getValues({ request: { filters: {}, order: "id-asc", page: 1 } });
  });

  const apiEndpoint = getApiEndpoint("Admin-Panel");
  const columns = [
    textColumn({ id: 0, target: "id", title: null, details: false, headerLeft: true }),
    textColumn({ id: 1, target: "prop1", title: null, details: false, headerLeft: true }),
    textColumn({ id: 2, target: "prop2", title: null, details: false, headerLeft: true }),
  ];


  const orders = {
    ["id-asc"]: "ID ↑",
    ["id-desc"]: "ID ↓",
    ["p1-asc"]: "Prop 1 ↑",
    ["p1-desc"]: "Prop 1 ↓",
    ["p2-asc"]: "Prop 2 ↑",
    ["p2-desc"]: "Prop 2 ↓",
  };
  return (
    <Stack spacing={1} width="100%">
      <Typography variant="h5">Admin-Panel API Endpoint: {apiEndpoint}</Typography>
    </Stack>
  );
}
