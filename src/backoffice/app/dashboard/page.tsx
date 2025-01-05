import { Suspense } from "react";
import Layout from "../../components/layout";
import { dashboardClient } from "../../lib/dashboard-client";
import DashboardContent from "./dashboard-content";

export default async function Dashboard() {
  const [metrics, metricDefinitions, chartData] = await Promise.all([
    dashboardClient.getMetrics(),
    dashboardClient.getMetricDefinitions(),
    dashboardClient.getChartData(),
  ]);

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Dashboard</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <DashboardContent
          metrics={metrics}
          metricDefinitions={metricDefinitions}
          chartData={chartData}
        />
      </Suspense>
    </Layout>
  );
}
