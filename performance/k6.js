import http from 'k6/http';
export const options = {
  vus: 2000,
  duration: '20s',
};

const getRandomInt = (min, max) => {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

export default function () {
  const id = getRandomInt(1, 1000);
  http.get('http://localhost:5170/cart/' + id);
}