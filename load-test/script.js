import http from "k6/http";
import { check } from "k6";
import { uuidv4 } from "https://jslib.k6.io/k6-utils/1.4.0/index.js";

export let options = {
  vus: 10,
  duration: "30s",
};

Array.prototype.random = function () {
  return this[Math.floor(Math.random() * this.length)];
};

const BASE_URL = "http://localhost:8080";
let guidArr = [];

export default function () {

  // POST request to set cache
  const payload = JSON.stringify({
    value: uuidv4(),
  });
  const headers = { "Content-Type": "application/json" };
  const rand = Math.floor(Math.random() * (300 - 1)) + 1;
  const params = { duration: rand };
  const setRes = http.post(`${BASE_URL}/cache/set?duration=${rand}`, payload, { headers });
  check(setRes, {
    "set cache status is 200": (r) => r.status === 200,
  });
  guidArr.push(setRes.json().key);
  console.log(setRes.json().key);

  // GET request to get cache
  const guid = guidArr.random();
  const getRes = http.get(`${BASE_URL}/cache/get/${guid}`);
  check(getRes, {
    "get cache status is 200": (r) => r.status === 200,
  });

  // // DELETE request to remove cache
  // for (let id of guidArr) {
  //   console.log(`Deleting cache with id: ${id}`);
  //   const deleteRes = http.del(`${BASE_URL}/cache/remove/${id}`);
  //   check(deleteRes, {
  //     "get cache status is 200": (r) => r.status === 200,
  //   });
  // }
}

export function teardown(data) {
  const getCountRes = http.get(`${BASE_URL}/cache/count`);
  check(getCountRes, {
    "get cache status is 200": (r) => r.status === 200,
  });
  console.log(`Total cache count: ${getCountRes.body}`);
}