'use client'

import { useState } from 'react'
import { DateRangePicker } from '../components/date-range-picker'
import { OrdersChart } from '../components/orders-chart'
import { MetricCards } from '../components/metric-cards'
import { CountrySelector } from '../components/country-selector'
import { CountryData, Metric, ChartDataPoint, CountryName } from '../lib/dashboard-client'

interface DashboardContentProps {
  metrics: { [key in CountryName]: CountryData }
  metricDefinitions: Metric[]
  chartData: ChartDataPoint[]
}

export default function DashboardContent({ metrics, metricDefinitions, chartData }: DashboardContentProps) {
  const [activeCountries, setActiveCountries] = useState<{ [key in CountryName]: boolean }>({
    Sweden: true,
    Finland: true,
    Norway: true,
    Denmark: true,
  })

  const handleCountryToggle = (country: CountryName) => {
    setActiveCountries(prev => ({ ...prev, [country]: !prev[country] }))
  }

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
  )
}

