# WeatherApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiWeatherLocationPost**](WeatherApi.md#apiweatherlocationpost) | **POST** /api/Weather/location |  |



## apiWeatherLocationPost

> WeatherLocationResponse apiWeatherLocationPost()



### Example

```ts
import {
  Configuration,
  WeatherApi,
} from '';
import type { ApiWeatherLocationPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new WeatherApi();

  try {
    const data = await api.apiWeatherLocationPost();
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters

This endpoint does not need any parameter.

### Return type

[**WeatherLocationResponse**](WeatherLocationResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

