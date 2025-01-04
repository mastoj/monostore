// instrumentation.ts

import { registerOTel } from "@vercel/otel";

export async function register() {
  if (process.env.NEXT_RUNTIME === "nodejs") {
    const { spanProcessors } = await import("./instrumentation.node");
    registerOTel({ spanProcessors: spanProcessors });
  }
}
