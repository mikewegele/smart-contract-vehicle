# NtsGeometryServices


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**geometryOverlay** | **object** |  | [optional] [default to undefined]
**geometryRelate** | **object** |  | [optional] [default to undefined]
**coordinateEqualityComparer** | **object** |  | [optional] [default to undefined]
**defaultSRID** | **number** |  | [optional] [readonly] [default to undefined]
**defaultCoordinateSequenceFactory** | [**CoordinateSequenceFactory**](CoordinateSequenceFactory.md) |  | [optional] [default to undefined]
**defaultPrecisionModel** | [**PrecisionModel**](PrecisionModel.md) |  | [optional] [default to undefined]
**defaultElevationModel** | [**ElevationModel**](ElevationModel.md) |  | [optional] [default to undefined]

## Example

```typescript
import { NtsGeometryServices } from './api';

const instance: NtsGeometryServices = {
    geometryOverlay,
    geometryRelate,
    coordinateEqualityComparer,
    defaultSRID,
    defaultCoordinateSequenceFactory,
    defaultPrecisionModel,
    defaultElevationModel,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
