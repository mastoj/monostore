// Baseline performance test focusing only on grain communication without database access
import http from 'k6/http';
import { sleep, check } from 'k6';
import { Trend, Rate } from 'k6/metrics';

// Custom metrics
const grainCallTrend = new Trend('grain_call_duration');
const grainCallRate = new Rate('grain_call_rate');
const grainCallFailRate = new Rate('grain_call_fail_rate');

export const options = {
  // Different load testing scenarios
  scenarios: {
    // Constant load test
    constant_load: {
      executor: 'constant-vus',
      vus: 50,
      duration: '30s',
      gracefulStop: '5s',
    },
    // Ramping load test (gradually increase users)
    ramping_load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 20 },
        { duration: '20s', target: 100 },
        { duration: '20s', target: 100 },
        { duration: '10s', target: 0 },
      ],
      gracefulStop: '5s',
      startTime: '30s',
    },
    // Spike test
    spike_test: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '5s', target: 0 },
        { duration: '5s', target: 200 },
        { duration: '5s', target: 0 },
      ],
      gracefulStop: '5s',
      startTime: '90s',
    },
  },
  thresholds: {
    // Define SLOs (Service Level Objectives)
    'http_req_duration': ['p(95)<50'], // 95% of requests should be below 50ms
    'grain_call_duration': ['p(95)<30'], // 95% of grain calls should be below 30ms
    'grain_call_fail_rate': ['rate<0.01'], // Less than 1% failure rate
    'http_req_failed': ['rate<0.01'], // Less than 1% HTTP failure rate
  },
};

// Shared pool of cart IDs to test with
const cartIds = [];

// Setup function to create some carts for testing
export function setup() {
  console.log('Creating test carts for performance baseline testing');
  
  // Create 10 carts that we'll use for testing
  for (let i = 0; i < 10; i++) {
    const sessionId = Math.floor(Math.random() * 10000000).toString();
    
    const response = http.post('http://localhost:5170/cart', JSON.stringify({
      operatingChain: "OCSEELG"
    }), {
      headers: {
        'Content-Type': 'application/json',
      },
      cookies: {
        'session-id': sessionId,
      }
    });
    
    if (response.status === 200) {
      const bodyJson = response.json();
      cartIds.push(bodyJson.data.id);
    }
  }
  
  console.log(`Created ${cartIds.length} test carts`);
  return { cartIds };
}

// Main test function
export default function (data) {
  // Get a random cart ID from the pool
  const cartId = data.cartIds[Math.floor(Math.random() * data.cartIds.length)];
  
  // Call our performance test endpoint
  const startTime = new Date().getTime();
  const response = http.get(`http://localhost:5170/cart/performance-test/${cartId}`);
  const endTime = new Date().getTime();
  
  // Track timing metrics
  const callDuration = endTime - startTime;
  grainCallTrend.add(callDuration);
  grainCallRate.add(1);
  grainCallFailRate.add(response.status !== 200);
  
  // Check response
  const success = check(response, {
    'status is 200': (r) => r.status === 200,
    'response has result data': (r) => r.json().result && r.json().result.data,
    'response includes duration': (r) => r.json().durationMs !== undefined,
  });
  
  if (!success) {
    console.error(`Failed call to performance-test endpoint: ${response.status} ${response.body}`);
  }
  
  // Compare with regular database-dependent endpoint for the same cart
  const regularResponse = http.get(`http://localhost:5170/cart/${cartId}`);
  
  // Allow system to recover between requests
  sleep(0.2);
}

// Teardown function (optional)
export function teardown(data) {
  console.log(`Baseline performance test completed with ${data.cartIds.length} test carts`);
}