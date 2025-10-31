# AlarmSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiAlarmSettingsGet**](AlarmSettingsApi.md#apialarmsettingsget) | **GET** /api/AlarmSettings |  |
| [**apiAlarmSettingsPut**](AlarmSettingsApi.md#apialarmsettingsput) | **PUT** /api/AlarmSettings |  |
| [**apiAlarmSettingsStatusGet**](AlarmSettingsApi.md#apialarmsettingsstatusget) | **GET** /api/AlarmSettings/status |  |



## apiAlarmSettingsGet

> UserSettingsDto apiAlarmSettingsGet()



### Example

```ts
import {
  Configuration,
  AlarmSettingsApi,
} from '';
import type { ApiAlarmSettingsGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmSettingsApi();

  try {
    const data = await api.apiAlarmSettingsGet();
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

[**UserSettingsDto**](UserSettingsDto.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiAlarmSettingsPut

> apiAlarmSettingsPut(userSettingsDto)



### Example

```ts
import {
  Configuration,
  AlarmSettingsApi,
} from '';
import type { ApiAlarmSettingsPutRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmSettingsApi();

  const body = {
    // UserSettingsDto (optional)
    userSettingsDto: ...,
  } satisfies ApiAlarmSettingsPutRequest;

  try {
    const data = await api.apiAlarmSettingsPut(body);
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
| **userSettingsDto** | [UserSettingsDto](UserSettingsDto.md) |  | [Optional] |

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


## apiAlarmSettingsStatusGet

> AlarmStatusDto apiAlarmSettingsStatusGet()



### Example

```ts
import {
  Configuration,
  AlarmSettingsApi,
} from '';
import type { ApiAlarmSettingsStatusGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AlarmSettingsApi();

  try {
    const data = await api.apiAlarmSettingsStatusGet();
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

[**AlarmStatusDto**](AlarmStatusDto.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

