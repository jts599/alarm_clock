
# AlarmStatus


## Properties

Name | Type
------------ | -------------
`currentTime` | Date
`isLightCurrentlyOn` | boolean
`currentColor` | [Color](Color.md)
`nextAlarmTime` | Date
`alarmTime` | string
`transitionMinutes` | number
`holdOnMinutes` | number
`activeDays` | Array&lt;string&gt;

## Example

```typescript
import type { AlarmStatus } from ''

// TODO: Update the object below with actual values
const example = {
  "currentTime": null,
  "isLightCurrentlyOn": null,
  "currentColor": null,
  "nextAlarmTime": null,
  "alarmTime": null,
  "transitionMinutes": null,
  "holdOnMinutes": null,
  "activeDays": null,
} satisfies AlarmStatus

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as AlarmStatus
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


