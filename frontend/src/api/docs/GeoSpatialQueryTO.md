# GeoSpatialQueryTO


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**userLocation** | [**Point**](Point.md) |  | [default to undefined]
**maxDistance** | **number** |  | [default to undefined]
**allowedManufactures** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**allowedModels** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**allowedTrims** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**allowedFueltypes** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**allowedDrivetrains** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**minRemainingReach** | **number** |  | [optional] [default to undefined]
**minSeats** | **number** |  | [optional] [default to undefined]
**maxSeats** | **number** |  | [optional] [default to undefined]
**minPricePerMinute** | **number** |  | [optional] [default to undefined]
**maxPricePerMinute** | **number** |  | [optional] [default to undefined]

## Example

```typescript
import { GeoSpatialQueryTO } from './api';

const instance: GeoSpatialQueryTO = {
    userLocation,
    maxDistance,
    allowedManufactures,
    allowedModels,
    allowedTrims,
    allowedFueltypes,
    allowedDrivetrains,
    minRemainingReach,
    minSeats,
    maxSeats,
    minPricePerMinute,
    maxPricePerMinute,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
