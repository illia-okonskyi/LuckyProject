import { useCallback } from "react";
import { isNil } from "lodash";

export function useServerRequest({
  queryProvider,
  handler = null,
  queryProviderDeps = [],
  deps = []
}) {
  // eslint-disable-next-line react-hooks/exhaustive-deps
  queryProvider = useCallback(queryProvider, queryProviderDeps);

  function serverRequest(ctx = null) {
    const f = (ctx, q) => serverRequestWrapper(ctx, q, handler);

    const query = queryProvider(ctx);
    if (query !== null) {
      f(ctx, query);
    }
  }

  // eslint-disable-next-line react-hooks/exhaustive-deps
  return useCallback(serverRequest, [queryProvider, handler, ...deps]);
}

async function serverRequestWrapper(ctx, query, handler) {
  try {
    handleResult(handler, { ctx: ctx, status: "pending" });
    const response = await query;
    handleResult(handler, {
      ctx: ctx, 
      status: "success",
      statusCode: response.status,
      data: response.data,
      lpApiPayload: response?.data?.lpApiPayload
    });
  }
  catch (e) {
    if (e.response) {
      if (!isNil(e.response.data.lpApiError)) {
        handleResult(handler, {
          ctx: ctx, 
          status: "lpApiError",
          statusCode: e.response.status,
          lpApiError: e.response.data.lpApiError
        });
        return;
      }

      handleResult(handler, {
        ctx: ctx, 
        status: "httpError",
        statusCode: e.response.status,
        data: e.response.data
      });
      return;
    }

    console.log("Server Hook Request Error", e);
    handleResult(handler, {
      ctx: ctx, 
      status: "error",
      error: e
    });
  }
}

function handleResult(handler, result) {
  if (handler !== null) {
    handler(result);
  }
}
