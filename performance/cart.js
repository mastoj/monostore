import http from "k6/http";
export const options = {
  vus: 64,
  duration: "60s",
};

const getRandomInt = (min, max) => {
  return Math.floor(Math.random() * (max - min + 1)) + min;
};

const getRandomUUID = () => {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c === "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
};

const randomUUIDS = Array.from({ length: 5000 }, () => getRandomUUID());

const createdCarts = {};

const getRandomSessionId = () => {
  return getRandomInt(1, 10000000).toString();
};

const baseUrls = {
  docker: "http://localhost:8001",
  local: "http://localhost:5170",
  prod: "https://monostore-api.whiteground-32f83688.northeurope.azurecontainerapps.io",
};
const environment = "docker";
const baseUrl = baseUrls[environment];

export default function () {
  // const id = randomUUIDS[getRandomInt(0, randomUUIDS.length - 1)];
  // Console log the count of created carts
  // if(!createdCarts[id]) {
  const response = http.post(
    `${baseUrl}/cart`,
    JSON.stringify({
      operatingChain: "OCSEELG",
    }),
    {
      headers: {
        "Content-Type": "application/json",
      },
      cookies: {
        "session-id": getRandomSessionId(),
      },
    }
  );
  const bodyJson = response.json();
  const id = bodyJson.data.id;
  http.post(
    `${baseUrl}/cart/` + id + "/items",
    JSON.stringify({
      cartId: id,
      operatingChain: "OCSEELG",
      productId: "209984",
    }),
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
  createdCarts[id] = true;
  // }
  http.put(
    `${baseUrl}/cart/` + id + "/items/" + "209984",
    JSON.stringify({
      productId: "209984",
      quantity: 3,
    }),
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
  http.get(`${baseUrl}/cart/` + id);

  // Checkout cart
  http.post(
    `${baseUrl}/checkout`,
    JSON.stringify({
      cartId: id,
      operatingChain: "OCSEELG",
    }),
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
}
