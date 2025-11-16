# AlarmStateApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiAlarmStateNumberOfActiveBulbsGet**](AlarmStateApi.md#apialarmstatenumberofactivebulbsget) | **GET** /api/AlarmState/number-of-active-bulbs |  |
| [**apiAlarmStateRemoveOverrideDelete**](AlarmStateApi.md#apialarmstateremoveoverridedelete) | **DELETE** /api/AlarmState/remove-override |  |
| [**apiAlarmStateStateSummaryGet**](AlarmStateApi.md#apialarmstatestatesummaryget) | **GET** /api/AlarmState/state-summary |  |
| [**apiAlarmStateTurnOnUntilPost**](AlarmStateApi.md#apialarmstateturnonuntilpost) | **POST** /api/AlarmState/turn-on-until |  |



## apiAlarmStateNumberOfActiveBulbsGet

> number apiAlarmStateNumberOfActiveBulbsGet()



### Example

```ts
import {
  Configuration,
  AlarmStateApi,
} from '';
import type { ApiAlarmStateNumberOfActiveBulbsGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmStateApi();

  try {
    const data = await api.apiAlarmStateNumberOfActiveBulbsGet();
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

**number**

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


## apiAlarmStateRemoveOverrideDelete

> apiAlarmStateRemoveOverrideDelete(removeAlarmOverrideRequest)



### Example

```ts
import {
  Configuration,
  AlarmStateApi,
} from '';
import type { ApiAlarmStateRemoveOverrideDeleteRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmStateApi();

  const body = {
    // RemoveAlarmOverrideRequest (optional)
    removeAlarmOverrideRequest: ...,
  } satisfies ApiAlarmStateRemoveOverrideDeleteRequest;

  try {
    const data = await api.apiAlarmStateRemoveOverrideDelete(body);
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
| **removeAlarmOverrideRequest** | [RemoveAlarmOverrideRequest](RemoveAlarmOverrideRequest.md) |  | [Optional] |

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
| **404** | Not Found |  -  |
| **400** | Bad Request |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiAlarmStateStateSummaryGet

> AlarmStateSummary apiAlarmStateStateSummaryGet()



### Example

```ts
import {
  Configuration,
  AlarmStateApi,
} from '';
import type { ApiAlarmStateStateSummaryGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmStateApi();

  try {
    const data = await api.apiAlarmStateStateSummaryGet();
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

[**AlarmStateSummary**](AlarmStateSummary.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **304** | Not Modified |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiAlarmStateTurnOnUntilPost

> LightOverrideState apiAlarmStateTurnOnUntilPost(createLightOverrideRequest)



### Example

```ts
import {
  Configuration,
  AlarmStateApi,
} from '';
import type { ApiAlarmStateTurnOnUntilPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmStateApi();

  const body = {
    // CreateLightOverrideRequest (optional)
    createLightOverrideRequest: ...,
  } satisfies ApiAlarmStateTurnOnUntilPostRequest;

  try {
    const data = await api.apiAlarmStateTurnOnUntilPost(body);
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
| **createLightOverrideRequest** | [CreateLightOverrideRequest](CreateLightOverrideRequest.md) |  | [Optional] |

### Return type

[**LightOverrideState**](LightOverrideState.md)

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

