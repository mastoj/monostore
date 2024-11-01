import http from 'k6/http';
export const options = {
  vus: 100,
  duration: '30s',
};

const getRandomInt = (min, max) => {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}


export default function () {
  const id = getRandomInt(0, 10000);
  // Console log the count of created carts
  http.get('http://localhost:5170/product/OCNOELK/' + id);
}