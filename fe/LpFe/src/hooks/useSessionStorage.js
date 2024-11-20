import { useState, useCallback } from "react";

const getSessionStorageValue = (key) => {
  const s = sessionStorage.getItem(key);
  if (s === null) {
    return null;
  }

  return JSON.parse(s);
};

const setSessionStorageValue = (key, value) => sessionStorage.setItem(key, JSON.stringify(value));

export const useSessionStorage = (key, defaultValue) => {
  const savedState = getSessionStorageValue(key);
  const [state, setState] = useState(savedState ?? defaultValue);

  const setSessionStorageState = useCallback((value) => {
    setSessionStorageValue(key, value);
    setState(value);
  },
  [key, setState]);

  return [state, setSessionStorageState];
};