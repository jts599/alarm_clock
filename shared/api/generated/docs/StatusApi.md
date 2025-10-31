# StatusApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiStatusColorGet**](StatusApi.md#apistatuscolorget) | **GET** /api/Status/color |  |
| [**apiStatusGet**](StatusApi.md#apistatusget) | **GET** /api/Status |  |
| [**apiStatusHealthGet**](StatusApi.md#apistatushealthget) | **GET** /api/Status/health |  |
| [**apiStatusLiveGet**](StatusApi.md#apistatusliveget) | **GET** /api/Status/live |  |



## apiStatusColorGet

> apiStatusColorGet()



### Example

```ts
import {
  Configuration,
  StatusApi,
} from '';
import type { ApiStatusColorGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StatusApi();

  try {
    const data = await api.apiStatusColorGet();
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


## apiStatusGet

> apiStatusGet()



### Example

```ts
import {
  Configuration,
  StatusApi,
} from '';
import type { ApiStatusGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StatusApi();

  try {
    const data = await api.apiStatusGet();
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


## apiStatusHealthGet

> apiStatusHealthGet()



### Example

```ts
import {
  Configuration,
  StatusApi,
} from '';
import type { ApiStatusHealthGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StatusApi();

  try {
    const data = await api.apiStatusHealthGet();
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


## apiStatusLiveGet

> apiStatusLiveGet()



### Example

```ts
import {
  Configuration,
  StatusApi,
} from '';
import type { ApiStatusLiveGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StatusApi();

  try {
    const data = await api.apiStatusLiveGet();
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

