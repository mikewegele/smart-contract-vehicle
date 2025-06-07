# Point


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**factory** | [**GeometryFactory**](GeometryFactory.md) |  | [optional] [default to undefined]
**userData** | **any** |  | [optional] [default to undefined]
**srid** | **number** |  | [optional] [default to undefined]
**precisionModel** | [**PrecisionModel**](PrecisionModel.md) |  | [optional] [default to undefined]
**numGeometries** | **number** |  | [optional] [readonly] [default to undefined]
**isSimple** | **boolean** |  | [optional] [readonly] [default to undefined]
**isValid** | **boolean** |  | [optional] [readonly] [default to undefined]
**area** | **number** |  | [optional] [readonly] [default to undefined]
**length** | **number** |  | [optional] [readonly] [default to undefined]
**centroid** | [**Point**](Point.md) |  | [optional] [default to undefined]
**interiorPoint** | [**Point**](Point.md) |  | [optional] [default to undefined]
**pointOnSurface** | [**Point**](Point.md) |  | [optional] [default to undefined]
**envelope** | [**Geometry**](Geometry.md) |  | [optional] [default to undefined]
**envelopeInternal** | [**Envelope**](Envelope.md) |  | [optional] [default to undefined]
**isRectangle** | **boolean** |  | [optional] [readonly] [default to undefined]
**coordinateSequence** | [**CoordinateSequence**](CoordinateSequence.md) |  | [optional] [default to undefined]
**coordinates** | [**Array&lt;Coordinate&gt;**](Coordinate.md) |  | [optional] [readonly] [default to undefined]
**numPoints** | **number** |  | [optional] [readonly] [default to undefined]
**isEmpty** | **boolean** |  | [optional] [readonly] [default to undefined]
**dimension** | [**Dimension**](Dimension.md) |  | [optional] [default to undefined]
**boundaryDimension** | [**Dimension**](Dimension.md) |  | [optional] [default to undefined]
**x** | **number** |  | [optional] [default to undefined]
**y** | **number** |  | [optional] [default to undefined]
**coordinate** | [**Coordinate**](Coordinate.md) |  | [optional] [default to undefined]
**geometryType** | **string** |  | [optional] [readonly] [default to undefined]
**ogcGeometryType** | [**OgcGeometryType**](OgcGeometryType.md) |  | [optional] [default to undefined]
**boundary** | [**Geometry**](Geometry.md) |  | [optional] [default to undefined]
**z** | **number** |  | [optional] [default to undefined]
**m** | **number** |  | [optional] [default to undefined]

## Example

```typescript
import { Point } from './api';

const instance: Point = {
    factory,
    userData,
    srid,
    precisionModel,
    numGeometries,
    isSimple,
    isValid,
    area,
    length,
    centroid,
    interiorPoint,
    pointOnSurface,
    envelope,
    envelopeInternal,
    isRectangle,
    coordinateSequence,
    coordinates,
    numPoints,
    isEmpty,
    dimension,
    boundaryDimension,
    x,
    y,
    coordinate,
    geometryType,
    ogcGeometryType,
    boundary,
    z,
    m,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
