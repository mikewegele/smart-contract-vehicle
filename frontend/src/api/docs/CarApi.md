# CarApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiCarQueryGet**](#apicarqueryget) | **GET** /api/Car/Query | |

# **apiCarQueryGet**
> Array<CarTO> apiCarQueryGet()


### Example

```typescript
import {
    CarApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CarApi(configuration);

const { status, data } = await apiInstance.apiCarQueryGet();
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

