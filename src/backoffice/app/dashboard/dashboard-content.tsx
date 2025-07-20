"use client";

import { use, useState } from "react";
import { CountrySelector } from "../../components/country-selector";
import { DateRangePicker } from "../../components/date-range-picker";
import { MetricCards } from "../../components/metric-cards";
import { OrdersChart } from "../../components/orders-chart";
import {
  ChartDataPoint,
  CountryData,
  CountryName,
  Metric,
} from "../../lib/dashboard-client";

interface DashboardContentProps {
  metrics: Promise<{ [key in CountryName]: CountryData }>;
  metricDefinitions: Promise<Metric[]>;
  chartData: Promise<ChartDataPoint[]>;
}

export default function DashboardContent({
  metrics: metricsPromise,
  metricDefinitions: metricDefinitionsPromise,
  chartData: chartDataPromise,
}: DashboardContentProps) {
  const [activeCountries, setActiveCountries] = useState<{
    [key in CountryName]: boolean;
  }>({
    Sweden: true,
    Finland: true,
    Norway: true,
    Denmark: true,
  });

  const metrics = use(metricsPromise);
  const metricDefinitions = use(metricDefinitionsPromise);
  const chartData = use(chartDataPromise);

  const handleCountryToggle = (country: CountryName) => {
    setActiveCountries((prev) => ({ ...prev, [country]: !prev[country] }));
  };

  return (
    <>
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <DateRangePicker />
        <CountrySelector
          activeCountries={activeCountries}
          onCountryToggle={handleCountryToggle}
        />
      </div>
      <div className="mt-6">
        <OrdersChart chartData={chartData} activeCountries={activeCountries} />
      </div>
      <div className="mt-6">
        <MetricCards
          metrics={metrics}
          metricDefinitions={metricDefinitions}
          activeCountries={activeCountries}
        />
      </div>
    </>
  );
}
