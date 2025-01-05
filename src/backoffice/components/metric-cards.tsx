"use client";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { CountryData, CountryName, Metric } from "../lib/dashboard-client";

interface MetricCardsProps {
  metrics: { [key in CountryName]: CountryData };
  metricDefinitions: Metric[];
  activeCountries: { [key in CountryName]: boolean };
}

export function MetricCards({
  metrics,
  metricDefinitions,
  activeCountries,
}: MetricCardsProps) {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      {metricDefinitions.map((metric) => (
        <Card key={metric.key}>
          <CardHeader>
            <CardTitle className="text-sm font-medium">
              {metric.title}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {(Object.keys(activeCountries) as CountryName[]).map(
                (country) =>
                  activeCountries[country] && (
                    <div key={country} className="flex justify-between">
                      <span className="text-sm">{country}:</span>
                      <span className="font-bold">
                        {metric.prefix}
                        {metric.key === "totalRevenue"
                          ? metrics[country][metric.key].toFixed(2)
                          : metrics[country][metric.key].toLocaleString()}
                      </span>
                    </div>
                  )
              )}
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
