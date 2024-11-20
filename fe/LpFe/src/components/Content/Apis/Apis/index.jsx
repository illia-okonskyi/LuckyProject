//#region Imports
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useForm, Controller } from "react-hook-form";

import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import TextField from "@mui/material/TextField";

import LpLoadGuard from "../../../Common/LpLoadGuard";
import { LpValidationSummary } from "../../../Common/LpValidation";
import LpDataList from "../../../Common/LpDataList/LpDataList";
import { textColumn, customColumn } from "../../../Common/LpDataList/columns";

import { useLpLayoutContext } from "../../../../hooks/contexts";
import { useMountEffect } from "../../../../hooks/useMountEffect";
import { useLpBreadcrumbs } from "../../../../features/breadcrumbs";

import { useApisApi } from "../../../../features/apis/useApisApi";
//#endregion

//#region InstallApi
function InstallApi({ install, installApi, t }) {
  const { handleSubmit, control } = useForm({
    defaultValues: {
      callbackUrl: "",
    },
  });

  const onSubmit = (data) => {
    installApi({ callbackUrl: data.callbackUrl });
  };

  return (
    <Stack spacing={1}>
      <Typography variant="h6">{t("lp-ui-shell:s.apis.install.title")}</Typography>
      <Stack direction="row" spacing={1} justifyContent="center" alignItems="center">

        <Controller
          name="callbackUrl"
          control={control}
          render={({ field: { onChange, onBlur, value, ref } }) => (
            <TextField
              fullWidth
              label={t("lp-ui-shell:s.apis.install.callbackUrl")}
              size="small"
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              inputRef={ref}
            />
          )}
        />

        <LpLoadGuard loading={install.pending}>
          <Button
            variant="contained"
            color="secondary"
            onClick={handleSubmit(onSubmit)}>
              {t("lp-ui-shell:s.apis.install")}
          </Button>
        </LpLoadGuard>
      </Stack>

      <LpValidationSummary validation={install.validation} />
    </Stack>
  );
}
//#endregion

//#region LpApis
function LpApis({ lpApis, install, uninstall, installLpApi, uninstallApi, t }) {
  const columns = [
    textColumn({ id: 0, target: "name", title: null, details: false, headerLeft: true }),
    customColumn({
      id: 1,
      target: "isInstalled",
      title: null,
      details: false,
      headerRight: true,
      renderer: (value, item) => {
        if (!value) {
          return (
            <LpLoadGuard loading={install.pending}>
              <Button
                variant="outlined"
                color="primary"
                onClick={() => installLpApi({ apiName: item.name })}
              >
                {t("lp-ui-shell:s.apis.install")}
              </Button>
            </LpLoadGuard>);
        }

        return (
          <LpLoadGuard loading={uninstall.pending}>
            <Button
              variant="outlined"
              color="error"
              onClick={() => uninstallApi({ apiId: item.id })}
            >
              {t("lp-ui-shell:s.apis.uninstall")}
            </Button>
          </LpLoadGuard>
        );
      }
    })
  ];

  return (
    <Stack spacing={1}>
      <Typography variant="h6">{t("lp-ui-shell:s.apis.lpApis")}</Typography>
      <LpLoadGuard loading={lpApis.pending}>
        <LpDataList items={lpApis.value} columns={columns} />
      </LpLoadGuard>
    </Stack>
  );
}
//#endregion

//#region AllApis
function AllApis({ apis, uninstall, uninstallApi, t }) {
  const columns = [
    textColumn({ id: 0, target: "name", title: t("lp-ui-shell:s.apis.name"), headerLeft: true }),
    textColumn({ id: 1, target: "description", title: t("lp-ui-shell:s.apis.description") }),
    customColumn({
      id: 2,
      target: null,
      title: null,
      details: false,
      headerRight: true,
      renderer: (_value, item) => (
        <LpLoadGuard loading={uninstall.pending}>
          <Button
            variant="outlined"
            color="error"
            onClick={() => uninstallApi({ apiId: item.id })}
          >
            {t("lp-ui-shell:s.apis.uninstall")}
          </Button>
        </LpLoadGuard>
      )
    })
  ];

  return (
    <Stack spacing={1}>
      <Typography variant="h6">{t("lp-ui-shell:s.apis.allApis")}</Typography>
      <LpLoadGuard loading={apis.pending}>
        <LpDataList items={apis.value} columns={columns} />
      </LpLoadGuard>
    </Stack>
  );
}
//#endregion

//#region Apis
export default function Apis() {
  const {
    state: { apis, lpApis, install, uninstall },
    actions: { getApis, getLpApis, installApi, installLpApi, uninstallApi }
  } = useApisApi();

  const { setHeader } = useLpLayoutContext();
  const { setCrumbs, buildCrumb } = useLpBreadcrumbs();
  const { t, i18n } = useTranslation(["lp-ui-shell"]);
  useEffect(() => {
    setHeader(t("lp-ui-shell:s.apis.header"));
    setCrumbs([buildCrumb("id", null, t("lp-ui-shell:s.apis.crumb"))]);
  }, [buildCrumb, i18n.language, setCrumbs, setHeader, t]);

  useMountEffect(() => {
    getApis();
    getLpApis();
  });

  return (
    <Stack spacing={1} width={{ xs: "100%", md: "50%" }} textAlign="center">
      <InstallApi install={install} installApi={installApi} t={t} />
      <Divider />
      <LpApis
        lpApis={lpApis}
        install={install}
        uninstall={uninstall}
        installLpApi={installLpApi}
        uninstallApi={uninstallApi}
        t={t} />
      <Divider />
      <AllApis
        apis={apis}
        uninstall={uninstall}
        uninstallApi={uninstallApi}
        t={t} />
    </Stack>
  );
}
//#endregion