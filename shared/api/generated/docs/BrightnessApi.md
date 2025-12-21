# BrightnessApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiBrightnessGet**](BrightnessApi.md#apibrightnessget) | **GET** /api/Brightness |  |
| [**apiBrightnessMaxGet**](BrightnessApi.md#apibrightnessmaxget) | **GET** /api/Brightness/max |  |
| [**apiBrightnessPost**](BrightnessApi.md#apibrightnesspost) | **POST** /api/Brightness |  |



## apiBrightnessGet

> apiBrightnessGet()



### Example

```ts
import {
  Configuration,
  BrightnessApi,
} from '';
import type { ApiBrightnessGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new BrightnessApi();

  try {
    const data = await api.apiBrightnessGet();
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
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiBrightnessMaxGet

> apiBrightnessMaxGet()



### Example

```ts
import {
  Configuration,
  BrightnessApi,
} from '';
import type { ApiBrightnessMaxGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new BrightnessApi();

  try {
    const data = await api.apiBrightnessMaxGet();
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
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiBrightnessPost

> apiBrightnessPost(setBrightnessRequest)



### Example

```ts
import {
  Configuration,
  BrightnessApi,
} from '';
import type { ApiBrightnessPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new BrightnessApi();

  const body = {
    // SetBrightnessRequest (optional)
    setBrightnessRequest: ...,
  } satisfies ApiBrightnessPostRequest;

  try {
    const data = await api.apiBrightnessPost(body);
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
| **setBrightnessRequest** | [SetBrightnessRequest](SetBrightnessRequest.md) |  | [Optional] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **400** | Bad Request |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

