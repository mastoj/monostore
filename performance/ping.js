import http from "k6/http";
export const options = {
  vus: 100,
  duration: "60s",
};

// docker, local, prod
const baseUrls = {
  docker: "http://localhost:8001",
  local: "http://localhost:5170",
  prod: "https://monostore-api.whiteground-32f83688.northeurope.azurecontainerapps.io",
};
const environment = "local";
const baseUrl = baseUrls[environment];

export default function () {
  // Console log the count of created carts
  http.get(`${baseUrl}/ping`);
}
