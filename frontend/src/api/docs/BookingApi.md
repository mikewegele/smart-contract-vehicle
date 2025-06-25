# BookingApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiBookingBlockCarPost**](#apibookingblockcarpost) | **POST** /api/Booking/BlockCar | |
|[**apiBookingGetAllReservationsGet**](#apibookinggetallreservationsget) | **GET** /api/Booking/GetAllReservations | |
|[**apiBookingReserveCarPost**](#apibookingreservecarpost) | **POST** /api/Booking/ReserveCar | |
|[**apiBookingUnlockCarPost**](#apibookingunlockcarpost) | **POST** /api/Booking/UnlockCar | |

# **apiBookingBlockCarPost**
> ReservationTO apiBookingBlockCarPost()


### Example

```typescript
import {
    BookingApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new BookingApi(configuration);

let carId: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiBookingBlockCarPost(
    carId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **carId** | [**string**] |  | (optional) defaults to undefined|


### Return type

**ReservationTO**

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

# **apiBookingGetAllReservationsGet**
> Array<ReservationTO> apiBookingGetAllReservationsGet()


### Example

```typescript
import {
    BookingApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new BookingApi(configuration);

const { status, data } = await apiInstance.apiBookingGetAllReservationsGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<ReservationTO>**

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

# **apiBookingReserveCarPost**
> string apiBookingReserveCarPost()


### Example

```typescript
import {
    BookingApi,
    Configuration,
    ReservationTO
} from './api';

const configuration = new Configuration();
const apiInstance = new BookingApi(configuration);

let reservationTO: ReservationTO; // (optional)

const { status, data } = await apiInstance.apiBookingReserveCarPost(
    reservationTO
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **reservationTO** | **ReservationTO**|  | |


### Return type

**string**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiBookingUnlockCarPost**
> apiBookingUnlockCarPost()


### Example

```typescript
import {
    BookingApi,
    Configuration,
    ReservationTO
} from './api';

const configuration = new Configuration();
const apiInstance = new BookingApi(configuration);

let reservationTO: ReservationTO; // (optional)

const { status, data } = await apiInstance.apiBookingUnlockCarPost(
    reservationTO
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **reservationTO** | **ReservationTO**|  | |


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

