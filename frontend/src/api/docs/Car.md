# Car


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**vin** | **string** |  | [default to undefined]
**owner** | [**User**](User.md) |  | [optional] [default to undefined]
**trim** | [**VehicleTrim**](VehicleTrim.md) |  | [optional] [default to undefined]
**currentPosition** | [**Point**](Point.md) |  | [default to undefined]
**remainingReach** | **number** |  | [optional] [default to undefined]
**colour** | **string** |  | [default to undefined]
**seatNumbers** | **number** |  | [default to undefined]
**pricePerMinute** | **number** |  | [default to undefined]

## Example

```typescript
import { Car } from './api';

const instance: Car = {
    id,
    vin,
    owner,
    trim,
    currentPosition,
    remainingReach,
    colour,
    seatNumbers,
    pricePerMinute,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
