import { countryData, metrics } from "../data/mock-data";

export interface CountryData {
  avgOrderValue: number;
  avgOrderItems: number;
  totalOrders: number;
  totalRevenue: number;
}

export interface Metric {
  title: string;
  key: keyof CountryData;
  prefix: string;
}

export type CountryName = "Sweden" | "Finland" | "Norway" | "Denmark";

export interface ChartDataPoint {
  hour: string;
  [country: string]: number | string;
}

export class DashboardClient {
  async getMetrics(): Promise<{ [key in CountryName]: CountryData }> {
    // In a real application, this would be an API call
    return countryData;
  }

  async getMetricDefinitions(): Promise<Metric[]> {
    // In a real application, this would be an API call
    return metrics as Metric[];
  }

  async getChartData(): Promise<ChartDataPoint[]> {
    // In a real application, this would be an API call
    const countries: CountryName[] = ["Sweden", "Finland", "Norway", "Denmark"];
    return Array.from({ length: 24 }, (_, i) => {
      const hour = `${String(i).padStart(2, "0")}:00`;
      const data: ChartDataPoint = { hour };
      countries.forEach((country) => {
        data[country] = Math.floor(Math.random() * 30);
      });
      return data;
    });
  }
}

export const dashboardClient = new DashboardClient();
