const serverPort = 5000;
let serverEndpoint = "https://localhost";
if (serverPort !== 443) {
  serverEndpoint += `:${serverPort}`;
}

window.CONFIG = {
  SERVER_ENDPOINT: serverEndpoint
};