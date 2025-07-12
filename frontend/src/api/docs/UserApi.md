# UserApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiUserDeleteDelete**](#apiuserdeletedelete) | **DELETE** /api/User/Delete | |
|[**apiUserGetGet**](#apiusergetget) | **GET** /api/User/Get | |
|[**apiUserGetUserByIdIdGet**](#apiusergetuserbyididget) | **GET** /api/User/GetUserById/{id} | |
|[**apiUserProfileGet**](#apiuserprofileget) | **GET** /api/User/Profile | |
|[**apiUserRegisterPost**](#apiuserregisterpost) | **POST** /api/User/Register | |
|[**apiUserUpdatePatch**](#apiuserupdatepatch) | **PATCH** /api/User/Update | |
|[**apiUserUpdateProfilePatch**](#apiuserupdateprofilepatch) | **PATCH** /api/User/UpdateProfile | |

# **apiUserDeleteDelete**
> apiUserDeleteDelete()


### Example

```typescript
import {
    UserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let id: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiUserDeleteDelete(
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

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserGetGet**
> apiUserGetGet()


### Example

```typescript
import {
    UserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let id: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiUserGetGet(
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

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserGetUserByIdIdGet**
> Array<UserTO> apiUserGetUserByIdIdGet()


### Example

```typescript
import {
    UserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiUserGetUserByIdIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**Array<UserTO>**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserProfileGet**
> UserTO apiUserProfileGet()


### Example

```typescript
import {
    UserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

const { status, data } = await apiInstance.apiUserProfileGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**UserTO**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserRegisterPost**
> apiUserRegisterPost()


### Example

```typescript
import {
    UserApi,
    Configuration,
    RegisterTO
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let registerTO: RegisterTO; // (optional)

const { status, data } = await apiInstance.apiUserRegisterPost(
    registerTO
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **registerTO** | **RegisterTO**|  | |


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserUpdatePatch**
> apiUserUpdatePatch()


### Example

```typescript
import {
    UserApi,
    Configuration,
    User
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let user: User; // (optional)

const { status, data } = await apiInstance.apiUserUpdatePatch(
    user
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **user** | **User**|  | |


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiUserUpdateProfilePatch**
> apiUserUpdateProfilePatch()


### Example

```typescript
import {
    UserApi,
    Configuration,
    UserProfileUpdateTO
} from './api';

const configuration = new Configuration();
const apiInstance = new UserApi(configuration);

let userProfileUpdateTO: UserProfileUpdateTO; // (optional)

const { status, data } = await apiInstance.apiUserUpdateProfilePatch(
    userProfileUpdateTO
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **userProfileUpdateTO** | **UserProfileUpdateTO**|  | |


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

