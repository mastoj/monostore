import http from 'k6/http';
export const options = {
  vus: 30,
  duration: '60s',
};

const getRandomInt = (min, max) => {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

const getRandomUUID = () => {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

const randomUUIDS = Array.from({length: 5000}, () => getRandomUUID());

const createdCarts = {};

export default function () {
  const id = randomUUIDS[getRandomInt(0, randomUUIDS.length - 1)];
  // Console log the count of created carts
  if(!createdCarts[id]) {
    http.post('http://localhost:5170/cart', JSON.stringify({
      cartId: id,
    }), {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    http.post('http://localhost:5170/cart/' + id + '/items', JSON.stringify({
      cartId: id,
      operatingChain: "OCSEELG",
      productId: "209984"
    }), {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    createdCarts[id] = true
  }
  http.post('http://localhost:5170/cart/' + id + '/items/' + "209984" + "/increase", JSON.stringify({
  }), {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  http.get('http://localhost:5170/cart/' + id);
}