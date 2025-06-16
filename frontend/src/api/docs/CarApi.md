# CarApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiCarGeoSpatialQueryPost**](#apicargeospatialquerypost) | **POST** /api/Car/GeoSpatialQuery | |
|[**apiCarGetAllCarsGet**](#apicargetallcarsget) | **GET** /api/Car/GetAllCars | |
|[**apiCarGetAutomotiveCompaniesGet**](#apicargetautomotivecompaniesget) | **GET** /api/Car/GetAutomotiveCompanies | |
|[**apiCarGetCarStatusesGet**](#apicargetcarstatusesget) | **GET** /api/Car/GetCarStatuses | |
|[**apiCarGetDrivetrainsGet**](#apicargetdrivetrainsget) | **GET** /api/Car/GetDrivetrains | |
|[**apiCarGetFueltypesGet**](#apicargetfueltypesget) | **GET** /api/Car/GetFueltypes | |
|[**apiCarGetModelsGet**](#apicargetmodelsget) | **GET** /api/Car/GetModels | |
|[**apiCarGetStatusGet**](#apicargetstatusget) | **GET** /api/Car/GetStatus | |
|[**apiCarGetTrimsGet**](#apicargettrimsget) | **GET** /api/Car/GetTrims | |
|[**apiCarReserveCarGet**](#apicarreservecarget) | **GET** /api/Car/ReserveCar | |

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
> Array<AutomotiveCompanyTO> apiCarGetAutomotiveCompaniesGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetAutomotiveCompaniesGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<AutomotiveCompanyTO>**

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

# **apiCarGetCarStatusesGet**
> Array<CarStatusTO> apiCarGetCarStatusesGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetCarStatusesGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<CarStatusTO>**

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

# **apiCarGetDrivetrainsGet**
> Array<DrivetrainTO> apiCarGetDrivetrainsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarGetDrivetrainsGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<DrivetrainTO>**

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
> Array<FuelTypeTO> apiCarGetFueltypesGet()


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

**Array<FuelTypeTO>**

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
> Array<VehicleModelTO> apiCarGetModelsGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let manufactureName: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetModelsGet(
    manufactureName
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **manufactureName** | [**string**] |  | (optional) defaults to undefined|


### Return type

**Array<VehicleModelTO>**

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

# **apiCarGetStatusGet**
> CarStatusTO apiCarGetStatusGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let carId: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarGetStatusGet(
    carId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **carId** | [**string**] |  | (optional) defaults to undefined|


### Return type

**CarStatusTO**

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

const { status, data } = await apiInstance.apiCarGetTrimsGet(
    modelName
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **modelName** | [**string**] |  | (optional) defaults to undefined|


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

# **apiCarReserveCarGet**
> CarTO apiCarReserveCarGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

let carId: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCarReserveCarGet(
    carId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **carId** | [**string**] |  | (optional) defaults to undefined|


### Return type

**CarTO**

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

