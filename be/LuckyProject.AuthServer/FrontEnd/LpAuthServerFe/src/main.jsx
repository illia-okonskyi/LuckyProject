import React from "react";
import ReactDOM from "react-dom/client";
import { HashRouter } from "react-router-dom";
import App from "./components/App";
import "./index.css";

import axios from "axios";
axios.defaults.headers.common["Accept"] = "text/plain, application/json, application/problem+json";

// NOTE: DEBUG CODE
// axios.interceptors.request.use(request => {
//   console.log('Starting Request', JSON.stringify(request, null, 2))
//   return request
// });

// axios.interceptors.response.use(response => {
//   console.log('Response:', JSON.stringify(response, null, 2))
//   return response
// });

const root = document.getElementById("root");
const renderedAt = root.getAttribute("data-rendered-at");

ReactDOM.createRoot(root).render(
  <React.StrictMode>
    <React.Suspense fallback={<div>Loading...</div>}>
      <HashRouter>
        <App renderedAt={renderedAt} />
      </HashRouter>
    </React.Suspense>
  </React.StrictMode>,
);
