'use client'

import { Checkbox } from "@/components/ui/checkbox"
import { Label } from "@/components/ui/label"
import { CountryName } from '../lib/dashboard-client'

interface CountrySelectorProps {
  activeCountries: { [key in CountryName]: boolean }
  onCountryToggle: (country: CountryName) => void
}

export function CountrySelector({ activeCountries, onCountryToggle }: CountrySelectorProps) {
  return (
    <div className="flex flex-wrap gap-4">
      {(Object.keys(activeCountries) as CountryName[]).map((country) => (
        <div key={country} className="flex items-center space-x-2">
          <Checkbox
            id={country}
            checked={activeCountries[country]}
            onCheckedChange={() => onCountryToggle(country)}
          />
          <Label htmlFor={country}>{country}</Label>
        </div>
      ))}
    </div>
  )
}

