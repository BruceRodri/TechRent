import http from 'k6/http';
import { check, sleep } from 'k6';
export const options = { vus: 5, duration: '45s', thresholds: { http_req_failed: ['rate<0.05'] } };
export default function () {
  const response = http.get('http://techrent/', {timeout: '3s'});
  check(response, { 'inicio disponible': r => r.status === 200 });
  sleep(0.2);
}
