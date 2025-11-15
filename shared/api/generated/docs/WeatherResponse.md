
# WeatherResponse


## Properties

Name | Type
------------ | -------------
`forecasts` | [Array&lt;SingleDayForecast&gt;](SingleDayForecast.md)

## Example

```typescript
import type { WeatherResponse } from ''

// TODO: Update the object below with actual values
const example = {
  "forecasts": null,
} satisfies WeatherResponse

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as WeatherResponse
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


