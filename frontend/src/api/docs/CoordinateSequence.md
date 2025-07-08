# CoordinateSequence


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**dimension** | **number** |  | [optional] [readonly] [default to undefined]
**measures** | **number** |  | [optional] [readonly] [default to undefined]
**spatial** | **number** |  | [optional] [readonly] [default to undefined]
**ordinates** | [**Ordinates**](Ordinates.md) |  | [optional] [default to undefined]
**hasZ** | **boolean** |  | [optional] [readonly] [default to undefined]
**hasM** | **boolean** |  | [optional] [readonly] [default to undefined]
**zOrdinateIndex** | **number** |  | [optional] [readonly] [default to undefined]
**mOrdinateIndex** | **number** |  | [optional] [readonly] [default to undefined]
**first** | [**Coordinate**](Coordinate.md) |  | [optional] [default to undefined]
**last** | [**Coordinate**](Coordinate.md) |  | [optional] [default to undefined]
**count** | **number** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { CoordinateSequence } from './api';

const instance: CoordinateSequence = {
    dimension,
    measures,
    spatial,
    ordinates,
    hasZ,
    hasM,
    zOrdinateIndex,
    mOrdinateIndex,
    first,
    last,
    count,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
