
# AlarmStateSummary


## Properties

Name | Type
------------ | -------------
`numberOfActiveBulbs` | number
`currentTime` | Date
`nextAlarmEvent` | [AlarmEventInfo](AlarmEventInfo.md)
`weatherForecast` | [WeatherResponse](WeatherResponse.md)

## Example

```typescript
import type { AlarmStateSummary } from ''

// TODO: Update the object below with actual values
const example = {
  "numberOfActiveBulbs": null,
  "currentTime": null,
  "nextAlarmEvent": null,
  "weatherForecast": null,
} satisfies AlarmStateSummary

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as AlarmStateSummary
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


