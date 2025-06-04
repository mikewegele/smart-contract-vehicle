# AddressApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiAddressDeleteDelete**](#apiaddressdeletedelete) | **DELETE** /api/Address/Delete | |
|[**apiAddressPostPost**](#apiaddresspostpost) | **POST** /api/Address/Post | |
|[**apiAddressUpdatePatch**](#apiaddressupdatepatch) | **PATCH** /api/Address/Update | |
|[**get**](#get) | **GET** /api/Address/Get | |

# **apiAddressDeleteDelete**
> apiAddressDeleteDelete()


### Example

```typescript
import {
    AddressApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AddressApi(configuration);

let id: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiAddressDeleteDelete(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**number**] |  | (optional) defaults to undefined|


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiAddressPostPost**
> apiAddressPostPost()


### Example

```typescript
import {
    AddressApi,
    Configuration,
    Address
} from './api';

const configuration = new Configuration();
const apiInstance = new AddressApi(configuration);

let address: Address; // (optional)

const { status, data } = await apiInstance.apiAddressPostPost(
    address
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **address** | **Address**|  | |


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiAddressUpdatePatch**
> apiAddressUpdatePatch()


### Example

```typescript
import {
    AddressApi,
    Configuration,
    Address
} from './api';

const configuration = new Configuration();
const apiInstance = new AddressApi(configuration);

let address: Address; // (optional)

const { status, data } = await apiInstance.apiAddressUpdatePatch(
    address
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **address** | **Address**|  | |


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **get**
> get()


### Example

```typescript
import {
    AddressApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AddressApi(configuration);

let id: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.get(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**number**] |  | (optional) defaults to undefined|


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

