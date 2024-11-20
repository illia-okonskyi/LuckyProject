import axios from "axios";

axios.defaults.timeout = 1000;
axios.defaults.timeoutErrorMessage = "lp-timeout";
axios.defaults.headers.common["Accept"] = "text/plain, application/json, application/problem+json";

axios.defaults.headers.post["Content-Type"] = "application/json";
axios.defaults.headers.put["Content-Type"] = "application/json";
axios.defaults.headers.delete["Content-Type"] = "application/json";

// NOTE: DEBUG
// axios.interceptors.request.use(request => {
//   console.log("Starting Request", JSON.stringify(request, null, 2));
//   return request;
// });

// axios.interceptors.response.use(response => {
//   console.log("Response:", JSON.stringify(response, null, 2));
//   return response;
// });
