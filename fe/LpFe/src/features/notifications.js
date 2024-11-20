import { useCallback, useEffect, useState } from "react";
import { createSelector, createSlice } from "@reduxjs/toolkit";
import { useDispatch, useSelector } from "react-redux";
import { HubConnectionState, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useMountEffect } from "../hooks/useMountEffect";

export const lpNotificationsSlice = createSlice({
  name: "notifications",
  initialState: {
    hubConnected: false,
    subscribers: { }
  },
  reducers: {
    setHubConnected: (state) => {
      state.hubConnected = true;
    },
    addSubscriber: (state, action) => {
      state.subscribers[action.payload.id] = {
        type: action.payload.type,
        hanler: action.payload.handler
      };
    },
    deleteSubscriber: (state, action) => {
      delete state.subscribers[action.payload];
    }
  },
  selectors: {
    handlers: createSelector(
      (state) => state.subscribers,
      (subscribers, type) => Object.entries(subscribers)
        .filter(([, s]) => s.type === type)
        .map(([, s]) => s.handler)
    ),
  }
});
export const lpNotificationsReducer = lpNotificationsSlice.reducer;

export function useLpNotifications({
  forceRenewAppAuth,
  onAuthorizeResult,
} = {}) {
  const [connection, setConnection] = useState(null);
  const dispatch = useDispatch();
  const actions = lpNotificationsSlice.actions;
  const selectors = lpNotificationsSlice.selectors;
  const hubConnected = useSelector((s) => s.notifications.hubConnected);
  
  useMountEffect(() => {
    setConnection(new HubConnectionBuilder()
      .withUrl(`${window.CONFIG.AUTH_SERVER_ENDPOINT}/notifications`)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Debug)
      .build());
  });

  useEffect(() => {
    if (!connection) {
      return;
    }

    if (!hubConnected && connection.state === HubConnectionState.Disconnected) {
      connection.start()
        .then(() => {
          connection.on(
            "Notification",
            (n) => {
              if (n.type === "lp.common.authorizeResult") {
                onAuthorizeResult(n.payload);
                return;
              }
        
              if (n.type === "lp.common.userChanged") {
                forceRenewAppAuth();
                return;
              }
        
              selectors.handlers(n.type).forEach((h) => h(n));
          });           
          dispatch(actions.setHubConnected());
        })
        .catch((e) => console.error("Failed to connect to Notifications Hub", e));
      return;
    }
  }, [
    connection,
    hubConnected,
    dispatch,
    actions,
    forceRenewAppAuth,
    onAuthorizeResult,
    selectors
  ]);

  const authorizeAsync = useCallback(
    (sessionId) => {
      if (connection?.state !== HubConnectionState.Connected) {
        return;
      }
      connection.send("Authorize", { sessionId });
    },
    [connection]
  );

  const subscribe = useCallback((type, handler) => {
    const id = crypto.randomUUID();
    dispatch(actions.addSubscriber({ id, type, handler}));
    return () => dispatch(actions.deleteSubscriber(id));
  }, [actions, dispatch]);  

  return {
    hubConnected,
    authorizeAsync,
    subscribe
  };
}