import { useState } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";

import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

import AuthLayout from "../../Common/AuthLayout";

import axios from "axios";

import { useTitle } from "../../../hooks/useTitle";
import { useServerRequest } from "../../../hooks/useServerRequest";

import { buildServerPath } from "../../../services/serverPathBuilder";

const ForgotPasswordStatusMessage = ({ state }) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const ctx = state.ctx;
  const [message, color] = {
    ["initial"]: [null, null],
    ["processing"]: [t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.statusMessage.processing"), "info"],
    ["error"]: [t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.statusMessage.error", { ctx }), "error"],
    ["reset-password"]: [null, null],
    ["reset-password-processing"]: [t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.statusMessage.processing"), "info"],
    ["reset-password-error"]: [t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.statusMessage.error", { ctx }), "error"],
    ["finish"]: [t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.statusMessage.finish"), "info"],
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

const ForgotPasswordForm = ({
  forgotPasswordRequest,
  state
}) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const [ userNameOrEmail, setUserNameOrEmail ] = useState("");
  const { register, handleSubmit } = useForm({
    values: {
      userNameOrEmail: userNameOrEmail,
    },
  });

  const onSubmit = data => {
    setUserNameOrEmail(data.userNameOrEmail);
    forgotPasswordRequest({ userNameOrEmail: data.userNameOrEmail });
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
          label={t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.userNameOrEmail")}
          autoComplete="email"
          autoFocus
          {...register("userNameOrEmail", { required: true })}
        />
        <Stack spacing={1}>
          <Button type="submit" variant="contained" fullWidth>
            {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.resetPassword")}
          </Button>
        </Stack>
        <ForgotPasswordStatusMessage sx={{ mt: 1, display: "block" }} state={state} />
      </Box>
    </>
  );
};

const ResetPasswordForm = ({
  requestId,
  codeResend,
  resetPassword,
  state,
}) => {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  const [ formData, setFormData ] = useState( { password: "", passwordRepeat: "", code: "" });
  const { register, handleSubmit } = useForm({
    values: formData,
  });

  const onSubmit = data => {
    setFormData(data);
    resetPassword({
      requestId,
      password: data.password,
      passwordRepeat: data.passwordRepeat,
      code: data.code
    });
  };
  const onResend = () => {
    codeResend({ requestId });
  };

  return (
    <>
      <Typography variant="body1" align="left">
        {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRules.title")}
      </Typography>
      <Typography variant="body2"  align="left">
        {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRules.1")}
      </Typography>
      <Typography variant="body2" align="left">
        {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRules.2")}
      </Typography>
      <Typography variant="body2"  align="left">
        {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRules.3")}
      </Typography>
      <Typography variant="body2"  align="left">
        {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRules.4")}
      </Typography>

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
          label={t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.password")}
          type="password"
          {...register("password", { required: true })}
        />

        <TextField
          margin="normal"
          required
          fullWidth
          label={t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.passwordRepeat")}
          type="password"
          {...register("passwordRepeat", { required: true })}
        />

        <TextField
          margin="normal"
          required
          fullWidth
          label={t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.code")}
          autoFocus
          {...register("code", { required: true })}
        />
        <Stack spacing={1}>
          <Button type="submit" variant="contained">
            {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.resetPassword")}
          </Button>
          <Button variant="contained" color="secondary" onClick={onResend}>
            {t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.form.codeResend")}
          </Button>
        </Stack>

        <ForgotPasswordStatusMessage sx={{ mt: 1, display: "block" }} state={state} />
      </Box>
    </>
  );
};

export default function ForgotPassword() {
  const { t } = useTranslation(["ui", "lp-authserver-errors"]);
  useTitle(t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.title"));
  const [state, setState] = useState({ state: "initial", ctx: null });
  const [requestId, setRequestId] = useState(null);

  const forgotPasswordRequest = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/forgot-password"),
      { userNameOrEmail: ctx.userNameOrEmail }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          setState({ state: "error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        setRequestId(r.lpApiPayload.requestId);
        setState({ state: "reset-password" });
        return;
      }

      console.log("!!! ForgotPasswordRequest", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [setState, setRequestId]
  });

  const resetPasswordCodeResend = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/reset-password-code-resend"),
      { requestId: ctx.requestId }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "reset-password-processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          setState({ state: "reset-password-error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        setState({ state: "reset-password", ctx: null });
        return;
      }

      console.log("!!! ResetPassword-Code-Resend", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [setState]
  });  

  const resetPassword = useServerRequest({
    queryProvider: ctx => axios.post(
      buildServerPath("/api/connect/reset-password"),
      {
        requestId: ctx.requestId,
        password: ctx.password,
        passwordRepeat: ctx.passwordRepeat,
        code: ctx.code
      }),
    handler: r => {
      if (r.status === "pending") {
        setState({ state: "reset-password-processing", ctx: null });
        return;
      }

      if (r.status === "success" && r.lpApiPayload !== null) {
        if (r.lpApiPayload.result === "Error") {
          setState({ state: "reset-password-error", ctx: t(r.lpApiPayload.errorMessage) });
          return;
        }

        setState({ state: "finish" });
        return;
      }

      console.log("!!! ResetPassword", r);
      setState({ state: "unknown", ctx: r });
    },
    deps: [setState]
  });  

  let children = null;
  if (state.state === "initial" || state.state === "error") {
    children = (
      <ForgotPasswordForm
        forgotPasswordRequest={forgotPasswordRequest}
        state={state}
      />
    );
  } else if (state.state === "reset-password" || state.state === "reset-password-error") {
    children = (
      <ResetPasswordForm
        requestId={requestId}
        codeResend={resetPasswordCodeResend}
        resetPassword={resetPassword}
        state={state}
      />
    );
  } else {
    children = (
      <ForgotPasswordStatusMessage state={state} />
    );
  }

  return (
    <>
      <AuthLayout
        request={t("ui:s.lp.authserver.fe.ui.pages.forgotPassword.request")}
        clientDisplayName="Lucky Project AuthServer">
        {children}
      </AuthLayout>
    </>
  );
}
