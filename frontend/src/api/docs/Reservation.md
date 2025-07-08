# Reservation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [readonly] [default to undefined]
**rentorId** | **string** |  | [optional] [readonly] [default to undefined]
**reservedCarId** | **string** |  | [optional] [readonly] [default to undefined]
**price** | **number** |  | [optional] [readonly] [default to undefined]
**reservationActive** | **boolean** |  | [optional] [readonly] [default to undefined]
**reservationTimeUTC** | **string** |  | [optional] [readonly] [default to undefined]
**blockageActive** | **boolean** |  | [optional] [readonly] [default to undefined]
**blockageTimeUTC** | **string** |  | [optional] [readonly] [default to undefined]
**blockchainTransactionId** | **string** |  | [optional] [default to undefined]
**reservationCompleted** | **boolean** |  | [optional] [readonly] [default to undefined]
**reservationCancelled** | **boolean** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { Reservation } from './api';

const instance: Reservation = {
    id,
    rentorId,
    reservedCarId,
    price,
    reservationActive,
    reservationTimeUTC,
    blockageActive,
    blockageTimeUTC,
    blockchainTransactionId,
    reservationCompleted,
    reservationCancelled,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
