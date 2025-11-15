
# SingleDayForecast


## Properties

Name | Type
------------ | -------------
`forecastDate` | Date
`dayOrNight` | [DayOrNight](DayOrNight.md)
`highTemperatureF` | number
`lowTemperatureF` | number
`shortForecast` | string
`iconName` | string

## Example

```typescript
import type { SingleDayForecast } from ''

// TODO: Update the object below with actual values
const example = {
  "forecastDate": null,
  "dayOrNight": null,
  "highTemperatureF": null,
  "lowTemperatureF": null,
  "shortForecast": null,
  "iconName": null,
} satisfies SingleDayForecast

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as SingleDayForecast
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


