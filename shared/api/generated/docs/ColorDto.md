
# ColorDto


## Properties

Name | Type
------------ | -------------
`r` | number
`g` | number
`b` | number
`kelvin` | number

## Example

```typescript
import type { ColorDto } from ''

// TODO: Update the object below with actual values
const example = {
  "r": null,
  "g": null,
  "b": null,
  "kelvin": null,
} satisfies ColorDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as ColorDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


