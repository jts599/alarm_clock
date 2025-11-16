# WeatherIconApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiWeatherIconSelectPost**](WeatherIconApi.md#apiweathericonselectpost) | **POST** /api/WeatherIcon/select |  |
| [**apiWeatherIconTestGet**](WeatherIconApi.md#apiweathericontestget) | **GET** /api/WeatherIcon/test |  |



## apiWeatherIconSelectPost

> apiWeatherIconSelectPost(weatherIconRequest)



### Example

```ts
import {
  Configuration,
  WeatherIconApi,
} from '';
import type { ApiWeatherIconSelectPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new WeatherIconApi();

  const body = {
    // WeatherIconRequest (optional)
    weatherIconRequest: ...,
  } satisfies ApiWeatherIconSelectPostRequest;

  try {
    const data = await api.apiWeatherIconSelectPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **weatherIconRequest** | [WeatherIconRequest](WeatherIconRequest.md) |  | [Optional] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiWeatherIconTestGet

> apiWeatherIconTestGet()



### Example

```ts
import {
  Configuration,
  WeatherIconApi,
} from '';
import type { ApiWeatherIconTestGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new WeatherIconApi();

  try {
    const data = await api.apiWeatherIconTestGet();
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

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

