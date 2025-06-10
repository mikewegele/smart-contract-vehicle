# CarApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiCarGeoSpatialQueryPost**](#apicargeospatialquerypost) | **POST** /api/Car/GeoSpatialQuery | |
|[**apiCarGetAllCarsGet**](#apicargetallcarsget) | **GET** /api/Car/GetAllCars | |
|[**apiCarGetAutomotiveCompaniesGet**](#apicargetautomotivecompaniesget) | **GET** /api/Car/GetAutomotiveCompanies | |
|[**apiCarGetDrivetrainsGet**](#apicargetdrivetrainsget) | **GET** /api/Car/GetDrivetrains | |
|[**apiCarGetDummyDataGet**](#apicargetdummydataget) | **GET** /api/Car/GetDummyData | |
|[**apiCarGetFueltypesGet**](#apicargetfueltypesget) | **GET** /api/Car/GetFueltypes | |
|[**apiCarGetModelsGet**](#apicargetmodelsget) | **GET** /api/Car/GetModels | |
|[**apiCarGetTrimsGet**](#apicargettrimsget) | **GET** /api/Car/GetTrims | |

# **apiCarGeoSpatialQueryPost**
> Array<CarTO> apiCarGeoSpatialQueryPost()


### Example

```typescript
import {
    CarApi,
    Configuration,
    GeoSpatialQueryTO
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let geoSpatialQueryTO: GeoSpatialQueryTO; // (optional)

const { status, data } = await apiInstance.apiCarGeoSpatialQueryPost(
    geoSpatialQueryTO
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **geoSpatialQueryTO** | **GeoSpatialQueryTO**|  | |


### Return type

**Array<CarTO>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCarGetAllCarsGet**
> Array<CarTO> apiCarGetAllCarsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetAllCarsGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<CarTO>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCarGetAutomotiveCompaniesGet**
> apiCarGetAutomotiveCompaniesGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let withId: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetAutomotiveCompaniesGet(
    withId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **withId** | [**boolean**] |  | (optional) defaults to undefined|


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

# **apiCarGetDrivetrainsGet**
> Array<string> apiCarGetDrivetrainsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let withId: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetDrivetrainsGet(
    withId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **withId** | [**boolean**] |  | (optional) defaults to undefined|


### Return type

**Array<string>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCarGetDummyDataGet**
> Array<CarTO> apiCarGetDummyDataGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetDummyDataGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<CarTO>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCarGetFueltypesGet**
> Array<string> apiCarGetFueltypesGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetFueltypesGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<string>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCarGetModelsGet**
> apiCarGetModelsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let manufactureName: string; // (optional) (default to undefined)
let withId: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetModelsGet(
    manufactureName,
    withId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **manufactureName** | [**string**] |  | (optional) defaults to undefined|
| **withId** | [**boolean**] |  | (optional) defaults to undefined|


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

# **apiCarGetTrimsGet**
> apiCarGetTrimsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let modelName: string; // (optional) (default to undefined)
let withId: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetTrimsGet(
    modelName,
    withId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **modelName** | [**string**] |  | (optional) defaults to undefined|
| **withId** | [**boolean**] |  | (optional) defaults to undefined|


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

