// instrumentation.node.js

import { AzureMonitorTraceExporter } from "@azure/monitor-opentelemetry-exporter";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-grpc";
import { BatchSpanProcessor } from "@opentelemetry/sdk-trace-node";

const spanProcessors = [new BatchSpanProcessor(new OTLPTraceExporter())];

if (process.env.APPLICATIONINSIGHTS_CONNECTION_STRING) {
  spanProcessors.push(new BatchSpanProcessor(new AzureMonitorTraceExporter()));
}

export { spanProcessors };
