# Geometry


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**factory** | [**GeometryFactory**](GeometryFactory.md) |  | [optional] [default to undefined]
**userData** | **any** |  | [optional] [default to undefined]
**srid** | **number** |  | [optional] [default to undefined]
**geometryType** | **string** |  | [optional] [readonly] [default to undefined]
**ogcGeometryType** | [**OgcGeometryType**](OgcGeometryType.md) |  | [optional] [default to undefined]
**precisionModel** | [**PrecisionModel**](PrecisionModel.md) |  | [optional] [default to undefined]
**coordinate** | [**Coordinate**](Coordinate.md) |  | [optional] [default to undefined]
**coordinates** | [**Array&lt;Coordinate&gt;**](Coordinate.md) |  | [optional] [readonly] [default to undefined]
**numPoints** | **number** |  | [optional] [readonly] [default to undefined]
**numGeometries** | **number** |  | [optional] [readonly] [default to undefined]
**isSimple** | **boolean** |  | [optional] [readonly] [default to undefined]
**isValid** | **boolean** |  | [optional] [readonly] [default to undefined]
**isEmpty** | **boolean** |  | [optional] [readonly] [default to undefined]
**area** | **number** |  | [optional] [readonly] [default to undefined]
**length** | **number** |  | [optional] [readonly] [default to undefined]
**centroid** | [**Point**](Point.md) |  | [optional] [default to undefined]
**interiorPoint** | [**Point**](Point.md) |  | [optional] [default to undefined]
**pointOnSurface** | [**Point**](Point.md) |  | [optional] [default to undefined]
**dimension** | [**Dimension**](Dimension.md) |  | [optional] [default to undefined]
**boundary** | [**Geometry**](Geometry.md) |  | [optional] [default to undefined]
**boundaryDimension** | [**Dimension**](Dimension.md) |  | [optional] [default to undefined]
**envelope** | [**Geometry**](Geometry.md) |  | [optional] [default to undefined]
**envelopeInternal** | [**Envelope**](Envelope.md) |  | [optional] [default to undefined]
**isRectangle** | **boolean** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { Geometry } from './api';

const instance: Geometry = {
    factory,
    userData,
    srid,
    geometryType,
    ogcGeometryType,
    precisionModel,
    coordinate,
    coordinates,
    numPoints,
    numGeometries,
    isSimple,
    isValid,
    isEmpty,
    area,
    length,
    centroid,
    interiorPoint,
    pointOnSurface,
    dimension,
    boundary,
    boundaryDimension,
    envelope,
    envelopeInternal,
    isRectangle,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
