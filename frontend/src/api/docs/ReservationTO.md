# ReservationTO


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**rentorId** | **string** |  | [default to undefined]
**reservedCarId** | **string** |  | [default to undefined]
**price** | **number** |  | [optional] [default to undefined]
**reservationTimeUTC** | **string** |  | [optional] [default to undefined]
**blockageTimeUTC** | **string** |  | [optional] [default to undefined]
**blockchainTransactionId** | **string** |  | [optional] [default to undefined]
**reservationCompleted** | **boolean** |  | [optional] [default to undefined]
**reservationCancelled** | **boolean** |  | [optional] [default to undefined]

## Example

```typescript
import { ReservationTO } from './api';

const instance: ReservationTO = {
    id,
    rentorId,
    reservedCarId,
    price,
    reservationTimeUTC,
    blockageTimeUTC,
    blockchainTransactionId,
    reservationCompleted,
    reservationCancelled,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
