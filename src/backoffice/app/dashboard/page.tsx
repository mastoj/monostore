import { Suspense } from "react";
import { dashboardClient } from "../../lib/dashboard-client";
import DashboardContent from "./dashboard-content";

export default async function Dashboard() {
  return (
    <>
      <h1 className="text-2xl font-bold mb-4">Dashboard</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <DashboardContent
          metrics={dashboardClient.getMetrics()}
          metricDefinitions={dashboardClient.getMetricDefinitions()}
          chartData={dashboardClient.getChartData()}
        />
      </Suspense>
    </>
  );
}
