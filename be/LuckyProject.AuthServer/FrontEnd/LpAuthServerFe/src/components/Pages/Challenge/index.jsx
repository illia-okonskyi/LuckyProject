import { isNil } from "lodash";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useSearchParams, Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

import AuthLayout from "../../Common/AuthLayout";

import axios from "axios";

import { useTitle } from "../../../hooks/useTitle";
import { useUserInfo } from "../../../hooks/useUserInfo";
import { useAuthDetails } from "../../../hooks/useAuthDetails";
import { useServerRequest } from "../../../hooks/useServerRequest";

import { buildServerPath } from "../../../services/serverPathBuilder";

const ChallengeStatusMessage = ({ state }) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const ctx = state.ctx;
  const [message, color] = {
    ["initial"]: [null, null],
    ["processing"]: [t("ui:s.lp.authserver.fe.ui.pages.challenge.statusMessage.signingIn"), "info"],
    ["error"]: [t("ui:s.lp.authserver.fe.ui.pages.challenge.statusMessage.error", { ctx }), "error"],
    ["two-factor"]: [null, null],
    ["two-factor-processing"]: [t("ui:s.lp.authserver.fe.ui.pages.challenge.statusMessage.signingIn"), "info"],
    ["two-factor-error"]: [t("ui:s.lp.authserver.fe.ui.pages.challenge.statusMessage.error", { ctx }), "error"],
    ["unknown"]: [JSON.stringify(ctx), "error"]
  }[state.state];

  if (message === null) {
    return null;
  }

  return (
    <>
      <Typography variant="overline" color={color}>{message}</Typography>
    </>
  );
};

const ChallengeForm = ({
  creds,
  setCreds,
  state,
  verifyChallenge,
  cancelAuthorize
}) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const { register, handleSubmit } = useForm({
    values: {
      userNameOrEmail: creds.userNameOrEmail ?? "",
      password: ""
    },
  });

  const onSubmit = data => {
    setCreds({ userNameOrEmail: data.userNameOrEmail, password: null });
    verifyChallenge({ creds: data });
  };

  return (
    <>
      <Box sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
      }}
        component="form"
        noValidate
        onSubmit={handleSubmit(onSubmit)}
      >
        <TextField
          margin="normal"
          required
          fullWidth
          label={t("ui:s.lp.authserver.fe.ui.pages.challenge.form.userNameOrEmail")}
          autoComplete="email"
          autoFocus
          {...register("userNameOrEmail", { required: true })}
        />
        <TextField
          margin="normal"
          required
          fullWidth
          label={t("ui:s.lp.authserver.fe.ui.pages.challenge.form.password")}
          type="password"
          autoComplete="current-password"
          {...register("password", { required: true })}
        />
        <Stack spacing={1}>
          <Button type="submit" variant="contained" fullWidth>
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.signIn")}
          </Button>
          <Button
            variant="contained"
            color="error"
            fullWidth
            onClick={cancelAuthorize}>
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.cancel")}
          </Button>
          <Link to="../forgotPassword" target="_blank" variant="body2">
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.forgotPassword")}
          </Link>
        </Stack>
        <ChallengeStatusMessage sx={{ mt: 1, display: "block" }} state={state} />
      </Box>
    </>
  );
};

const ChallengeTwoFactorForm = ({
  creds,
  twoFactorCreds,
  twoFactorResend,
  twoFactorVerifyChallenge,
  cancelAuthorize,
  state,
}) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const { register, handleSubmit } = useForm({
    values: { code: "" },
  });

  const onSubmit = data => {
    twoFactorVerifyChallenge({ creds, twoFactorCreds, code: data.code });
  };
  const onResend = () => {
    twoFactorResend({ twoFactorCreds });
  };

  return (
    <>
      <Box sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
      }}
        component="form"
        noValidate
        onSubmit={handleSubmit(onSubmit)}
      >
        <TextField
          margin="normal"
          required
          fullWidth
          label={t("ui:s.lp.authserver.fe.ui.pages.challenge.form.twoFactorCode")}
          autoFocus
          {...register("code", { required: true })}
        />
        <Stack spacing={1}>
          <Button type="submit" variant="contained" fullWidth>
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.signIn")}
          </Button>
          <Button variant="contained" color="secondary" onClick={onResend} fullWidth>
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.resendTwoFactor")}
          </Button>
          <Button
            variant="contained"
            color="error"
            fullWidth
            onClick={cancelAuthorize}>
            {t("ui:s.lp.authserver.fe.ui.pages.challenge.form.cancel")}
          </Button>
        </Stack>
        <ChallengeStatusMessage sx={{ mt: 1, display: "block" }} state={state} />
      </Box>
    </>
  );
};

export default function Challenge() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useTitle(t("ui:s.lp.authserver.fe.ui.pages.challenge.title"));
  const [state, setState] = useState({ state: "initial", ctx: null });
  const [userInfo, setUserInfo] = useUserInfo();
  const [searchParams] = useSearchParams();
  const getAuthDetails = useAuthDetails(searchParams);
  const authDetails = getAuthDetails();
  const [creds, setCreds] = useState({ userNameOrEmail: null, password: null });
  const [twoFactorCreds, setTwoFactorCreds] = useState({ userId: null, twoFactorRequestId: null });

  const finishAuthorize = (userId, consentRequired) => {
    searchParams.set("lp_uid", userId);
    if (!consentRequired) {
      searchParams.set("lp_cr", "yes");
    }
    window.location.href = buildServerPath("/api/connect/authorize-finish?") + searchParams;
  };

  const cancelAuthorize = () => {
    searchParams.set("lp_cancel", "yes");
    window.location.href = buildServerPath("/api/connect/authorize-finish?") + searchParams;
  };

  const verifyChallenge = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/authorize-challenge-verify"),
      {
        clientId: authDetails.clientId,
        ...ctx.creds
      }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          if (userInfo.isLoggedIn) {
            setUserInfo({
              isLoggedIn: false,
              userId: null
            });
            setState({ state: "initial", ctx: null });
            return;
          }

          setState({ state: "error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        if (r.lpApiPayload.result === "TwoFactor") {
          if (r.lpApiPayload.twoFactorRequestId === null) {
            setState({ state: "error", ctx: t(r.lpApiPayload.errorMessage) });
            return;
          }

          setTwoFactorCreds({
            userId: r.lpApiPayload.userId,
            twoFactorRequestId: r.lpApiPayload.twoFactorRequestId,
          });
          setState({ state: "two-factor", ctx: null });
          return;
        }

        setUserInfo({
          isLoggedIn: true,
          userId: r.lpApiPayload.userId
        });

        if (!isNil(r.ctx.creds.userId)) {
          finishAuthorize(r.lpApiPayload.userId, false);
          return;
        }

        return;
      }

      console.log("!!! Challenge-Verify", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [searchParams, setState, userInfo, setUserInfo, setTwoFactorCreds]
  });

  const twoFactorResend = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/authorize-challenge-two-factor-resend"),
      {
        clientId: authDetails.clientId,
        ...ctx.twoFactorCreds,
      }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "two-factor-processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          setState({ state: "two-factor-error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        setState({ state: "two-factor", ctx: null });
        return;
      }

      console.log("!!! Challenge-Two-Factor-Resend", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [setState]
  });

  const twoFactorVerifyChallenge = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/authorize-challenge-two-factor-verify"),
      {
        clientId: authDetails.clientId,
        ...ctx.twoFactorCreds,
        twoFactorCode: ctx.code
      }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "two-factor-processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          setState({ state: "two-factor-error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        setUserInfo({
          isLoggedIn: true,
          userId: r.lpApiPayload.userId
        });

        finishAuthorize(r.lpApiPayload.userId, true);
        return;
      }

      console.log("!!! Challenge-Two-Factor-Verify", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [setState, userInfo, setUserInfo]
  });

  if (state.state === "initial" && userInfo.isLoggedIn) {
    verifyChallenge({ creds: { userId: userInfo.userId } });
    return null;
  }

  let children = null;
  if (state.state === "initial" || state.state === "error") {
    children = (
      <ChallengeForm
        creds={creds}
        setCreds={setCreds}
        state={state}
        verifyChallenge={verifyChallenge}
        cancelAuthorize={cancelAuthorize}
      />
    );
  } else if (state.state === "two-factor" || state.state === "two-factor-error") {
    children = (
      <ChallengeTwoFactorForm
        creds={creds}
        twoFactorCreds={twoFactorCreds}
        twoFactorResend={twoFactorResend}
        twoFactorVerifyChallenge={twoFactorVerifyChallenge}
        cancelAuthorize={cancelAuthorize}
        state={state}
      />
    );
  } else {
    children = (
      <ChallengeStatusMessage state={state} />
    );
  }

  return (
    <>
      <AuthLayout
        request={t("ui:s.lp.authserver.fe.ui.pages.challenge.request")}
        clientDisplayName={authDetails.clientDisplayName}>
        {children}
      </AuthLayout>
    </>
  );
}
