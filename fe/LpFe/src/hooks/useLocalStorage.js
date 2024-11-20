import { useState, useCallback } from "react";

const getLocalStorageValue = (key) => {
  const s = localStorage.getItem(key);
  if (s === null) {
    return null;
  }

  return JSON.parse(s);
};

const setLocalStorageValue = (key, value) => localStorage.setItem(key, JSON.stringify(value));

export const useLocalStorage = (key, defaultValue) => {
  const savedState = getLocalStorageValue(key);
  const [state, setState] = useState(savedState ?? defaultValue);

  const setLocalStorageState = useCallback((value) => {
    setLocalStorageValue(key, value);
    setState(value);
  },
  [key, setState]);

  return [state, setLocalStorageState];
};