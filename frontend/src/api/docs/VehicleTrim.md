# VehicleTrim


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**name** | **string** |  | [default to undefined]
**model** | [**VehicleModel**](VehicleModel.md) |  | [optional] [default to undefined]
**cars** | [**Array&lt;Car&gt;**](Car.md) |  | [optional] [default to undefined]
**fuel** | [**FuelType**](FuelType.md) |  | [optional] [default to undefined]
**drivetrain** | [**Drivetrain**](Drivetrain.md) |  | [optional] [default to undefined]
**imagePath** | **string** |  | [default to undefined]

## Example

```typescript
import { VehicleTrim } from './api';

const instance: VehicleTrim = {
    id,
    name,
    model,
    cars,
    fuel,
    drivetrain,
    imagePath,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
