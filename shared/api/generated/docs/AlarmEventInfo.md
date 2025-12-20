
# AlarmEventInfo


## Properties

Name | Type
------------ | -------------
`nextEventDayOfWeek` | string
`nextEventTime` | string
`nextEventType` | [EventType](EventType.md)

## Example

```typescript
import type { AlarmEventInfo } from ''

// TODO: Update the object below with actual values
const example = {
  "nextEventDayOfWeek": null,
  "nextEventTime": null,
  "nextEventType": null,
} satisfies AlarmEventInfo

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as AlarmEventInfo
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


