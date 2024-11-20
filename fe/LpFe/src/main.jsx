import React from "react";
import ReactDOM from "react-dom/client";
import App from "./components/App";
import LpLoadSpinner from "./components/Common/LpLoadSpinner";
import { BrowserRouter } from "react-router-dom";

import "./tabSllepLock";
import "./index.css";
import "./net/config";

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <React.Suspense fallback={<LpLoadSpinner label="Loading Lucky Project..." />}>
      <BrowserRouter  future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true,
        }}>
        <App />
      </BrowserRouter>
    </React.Suspense>
  </React.StrictMode>
);
