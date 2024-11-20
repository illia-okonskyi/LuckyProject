import { useCallback, } from "react";
import { isNil } from "lodash";
import { useTranslation } from "react-i18next";
import { useAppAuthContext } from "../hooks/contexts";

export function useLpApiRequest({
  queryProvider,
  onPending = null,
  onSuccess = null,
  onError = null,
  onCancel = null
}) {
  const { t } = useTranslation(["lp-validation"]);
  const { getAcccessToken } = useAppAuthContext();
  const authHeaderProvider = useCallback(
    () => { return { "Authorization": getAcccessToken() }; },
    [getAcccessToken]);

  const handler = useCallback(
    (r) => {
      const status = r.status;
      const outerHandler = {
        ["pending"]: onPending,
        ["success"]: onSuccess,
        ["error"]: onError,
        ["cancel"]: onCancel
      }[status];
      if (outerHandler !== null) {
        outerHandler({ ctx: r.ctx, payload: r.payload });
      }
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [onPending, onSuccess, onError, onCancel].filter((d) => !!d)
  );

  const lpApiRequestAsync = useCallback(
    async function (query, ctx) {
      try {
        handleResult(handler, { ctx, status: "pending", payload: null });
        const response = await query;
        const payload = response.data?.lpApiPayload || null;
        handleResult(handler, {
          ctx,
          status: "success",
          payload: {
            statusCode: response.status,
            content: response.data,
            payload
          }
        });
      }
      catch (e) {
        if (e.name === "CanceledError") {
          handleResult(handler, { ctx, status: "cancel", payload: null });
          return;
        }

        if (e.response) {
          if (e.response.status === 401) {
            handleUnauthorizedError(handler, ctx);
            return;
          }

          if (!isNil(e.response.data.lpApiError)) {
            handleLpApiError(handler, ctx, e.response.status, e.response.data.lpApiError, t);
            return;
          }

          handleHttpError(handler, ctx, e.response.status, e.response?.data);
          return;
        }
        
        handleOtherError(handler, ctx, e);
      }
    },
    [handler, t]);

  return useCallback(function(ctx = null) {
      const query = queryProvider(authHeaderProvider, ctx);
      if (query !== null) {
        lpApiRequestAsync(query, ctx);
      }
    }, [queryProvider, authHeaderProvider, lpApiRequestAsync]);
}

//
// NOTE: Internals
//

function handleUnauthorizedError(handler, ctx) {
  handleResult(handler, buildErrorResult({ ctx, errorType: "client/unauthorized" }));
}

function handleLpApiError(handler, ctx, statusCode, error, t) {
  const type = error.type;

  // NOTE: Handle client-side errors
  if (type === "bad-request") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "client/bad-request",
      statusCode,
      message: error.message
    }));
    return;
  }

  if (type === "validation-error") {
    const vr = error.result;
    const cancelled = vr.isCancelled ? t("lp-validation:s.cancelled") : null;
    const errors = Object.fromEntries(
      Object.entries(Object.groupBy(vr.errors, (e) => e.path))
        .map(([key, value]) => [
          key,
          JSON.stringify(
            value.reduce((acc, e) => {
              const messages = e.code === "messages-only"
                ? e.messages
                : [buildValidationErrorMessage(e, t)];
              return [...acc, ...messages];
            },
            [])
          )
        ])
    );
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "client/validation",
      statusCode,
      validation: { cancelled, errors }
    }));
    return;
  }

  if (type === "access-denied") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "client/access-denied",
      statusCode,
      message: error.message
    }));
    return;
  }

  // NOTE: Handle server-side errors
  if (type === "internal-server-error") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/internal",
      statusCode,
      message: error.message
    }));
    return;
  }

  if (type === "not-implemented") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/not-implemented",
      statusCode,
      message: error.message
    }));
    return;
  }

  if (type === "bad-gateway") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/bad-gateway",
      statusCode,
      message: error.message
    }));
    return;
  }

  if (type === "service-unavailable") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/service-unavailable",
      statusCode,
      message: error.message
    }));
    return;
  }

  if (type === "gateway-timeout") {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/timeout",
      statusCode,
      message: error.message
    }));
    return;
  }

  console.error("useLpApiRequest.unexpectedLpApiError");
}

function handleHttpError(handler, ctx, statusCode, content) {
  if (statusCode === 400) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "client/bad-request",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 403) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "client/access-denied",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 500) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/internal",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 501) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/not-implemented",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 502) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/bad-gateway",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 503) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/service-unavailable",
      statusCode,
      content
    }));
    return;
  }

  if (statusCode === 504) {
    handleResult(handler, buildErrorResult({
      ctx,
      errorType: "server/timeout",
      statusCode,
      content
    }));
    return;
  }

  handleResult(handler, buildErrorResult({ ctx, errorType: "other/http", statusCode, content }));
}

function handleOtherError(handler, ctx, exception) {
  if (exception.message === "lp-timeout") {
    handleResult(handler, buildErrorResult({ ctx, errorType: "server/timeout" }));
    return;
  }

  console.error("useLpApiRequest.otherError", exception);
  handleResult(handler, buildErrorResult({ ctx, errorType: "other/exception", exception }));
}

function handleResult(handler, result) {
  if (handler !== null) {
    try {
      handler(result);
    } catch (e) {
      console.error("useLpApiRequest.handler.error", e);
    }
  }
}

function buildErrorResult({
  ctx,
  errorType,
  message = null,
  validation = null,
  statusCode = null,
  content = null,
  exception = null
}) {
  return {
    ctx,
    status: "error",
    payload: {
      errorType,
      message,
      validation,
      statusCode,
      content,
      exception
    }
  };
}


function buildValidationErrorMessage(e, t) {
  if (e.code === "not-null") {
    return t("lp-validation:s.errors.not-null");
  }
  if (e.code === "null") {
    return t("lp-validation:s.errors.null");
  }
  if (e.code === "not-empty") {
    return t("lp-validation:s.errors.not-empty");
  }
  if (e.code === "empty") {
    return t("lp-validation:s.errors.empty");
  }
  if (e.code === "not-equal") {
    return t("lp-validation:s.errors.not-equal", { target: e.details.target });
  }
  if (e.code === "equal") {
    return t("lp-validation:s.errors.equal", { target: e.details.target });
  }
  if (e.code === "length") {
    return t("lp-validation:s.errors.length", { min: e.details.min, max: e.details.max });
  }
  if (e.code === "min-length") {
    return t("lp-validation:s.errors.min-length", { target: e.details.target });
  }
  if (e.code === "max-length") {
    return t("lp-validation:s.errors.max-length", { target: e.details.target });
  }
  if (e.code === "less-than") {
    return t("lp-validation:s.errors.less-than", { target: e.details.target });
  }
  if (e.code === "less-than-or-equal") {
    return t("lp-validation:s.errors.less-than-or-equal", { target: e.details.target });
  }
  if (e.code === "greater-than") {
    return t("lp-validation:s.errors.greater-than", { target: e.details.target });
  }
  if (e.code === "greater-than-or-equal") {
    return t("lp-validation:s.errors.greater-than-or-equal", { target: e.details.target });
  }
  if (e.code === "regex") {
    return t("lp-validation:s.errors.regex", { target: e.details.target });
  }
  if (e.code === "email") {
    return t("lp-validation:s.errors.email");
  }
  if (e.code === "login") {
    return t("lp-validation:s.errors.login");
  }
  if (e.code === "pwd") {
    return t("lp-validation:s.errors.pwd");
  }
  if (e.code === "creds") {
    return t("lp-validation:s.errors.creds");
  }
  if (e.code === "creds") {
    return t("lp-validation:s.errors.creds");
  }
  if (e.code === "url") {
    return t("lp-validation:s.errors.url", { target: e.details.target });
  }
  if (e.code === "full-name") {
    return t("lp-validation:s.errors.full-name");
  }
  if (e.code === "permission") {
    return t("lp-validation:s.errors.permission");
  }
  if (e.code === "phone") {
    return t("lp-validation:s.errors.phone");
  }
  if (e.code === "credit-card") {
    return t("lp-validation:s.errors.credit-card");
  }
  if (e.code === "enum") {
    return t("lp-validation:s.errors.enum");
  }
  if (e.code === "exclusive-between") {
    return t("lp-validation:s.errors.exclusive-between", { min: e.details.min, max: e.details.max });
  }
  if (e.code === "inclusive-between") {
    return t("lp-validation:s.errors.exclusive-between", { min: e.details.min, max: e.details.max });
  }
  if (e.code === "precision-scale") {
    return t("lp-validation:s.errors.exclusive-between", { precision: e.details.precision, scale: e.details.scale });
  }
  if (e.code === "unique") {
    return e.details.target
      ? t("lp-validation:s.errors.unique")
      : t("lp-validation:s.errors.not-unique");
  }
  if (e.code === "not-found") {
    return t("lp-validation:s.errors.not-found");
  }
  if (e.code === "access-denied") {
    return t("lp-validation:s.errors.access-denied");
  }
  if (e.code === "access-denied") {
    return t("lp-validation:s.errors.access-denied");
  }
  if (e.code === "prefix") {
    return e.details.target2
      ? t("lp-validation:s.errors.prefixed", { target: e.details.target })
      : t("lp-validation:s.errors.not-prefixed", { target: e.details.target });
  }
  if (e.code === "suffix") {
    return e.details.target2
      ? t("lp-validation:s.errors.suffixed", { target: e.details.target })
      : t("lp-validation:s.errors.not-suffixed", { target: e.details.target });
  }
  if (e.code === "invalid") {
    return t("lp-validation:s.errors.invalid");
  }
  if (e.code === "unknown") {
    return t("lp-validation:s.errors.unknown");
  }

  // NOTE: Fallback
  return `${e.code}/${JSON.stringify(e.details)}`;
}