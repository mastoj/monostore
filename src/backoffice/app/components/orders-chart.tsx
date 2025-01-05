'use client'

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart"
import { ResponsiveContainer, AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts'
import { ChartDataPoint, CountryName } from '../lib/dashboard-client'

const colors = {
  Sweden: 'hsl(var(--chart-1))',
  Finland: 'hsl(var(--chart-2))',
  Norway: 'hsl(var(--chart-3))',
  Denmark: 'hsl(var(--chart-4))',
}

interface OrdersChartProps {
  chartData: ChartDataPoint[]
  activeCountries: { [key in CountryName]: boolean }
}

export function OrdersChart({ chartData, activeCountries }: OrdersChartProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Orders per Hour</CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <ChartContainer
          config={Object.fromEntries(
            Object.entries(colors).map(([country, color]) => [
              country,
              { label: country, color },
            ])
          )}
          className="h-[400px] w-full"
        >
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={chartData} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="hour" tick={{ fontSize: 12 }} tickFormatter={(value) => value.split(':')[0]} />
              <YAxis allowDecimals={false} domain={[0, 'auto']} tick={{ fontSize: 12 }} />
              <ChartTooltip content={<ChartTooltipContent />} />
              <Legend />
              {(Object.keys(colors) as CountryName[]).map((country) => (
                activeCountries[country] && (
                  <Area
                    key={country}
                    type="monotone"
                    dataKey={country}
                    stroke={colors[country]}
                    fill={colors[country]}
                    fillOpacity={0.3}
                  />
                )
              ))}
            </AreaChart>
          </ResponsiveContainer>
        </ChartContainer>
      </CardContent>
    </Card>
  )
}

