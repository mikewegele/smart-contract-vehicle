# User


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**userName** | **string** |  | [optional] [default to undefined]
**normalizedUserName** | **string** |  | [optional] [default to undefined]
**email** | **string** |  | [optional] [default to undefined]
**normalizedEmail** | **string** |  | [optional] [default to undefined]
**emailConfirmed** | **boolean** |  | [optional] [default to undefined]
**passwordHash** | **string** |  | [optional] [default to undefined]
**securityStamp** | **string** |  | [optional] [default to undefined]
**concurrencyStamp** | **string** |  | [optional] [default to undefined]
**phoneNumber** | **string** |  | [optional] [default to undefined]
**phoneNumberConfirmed** | **boolean** |  | [optional] [default to undefined]
**twoFactorEnabled** | **boolean** |  | [optional] [default to undefined]
**lockoutEnd** | **string** |  | [optional] [default to undefined]
**lockoutEnabled** | **boolean** |  | [optional] [default to undefined]
**accessFailedCount** | **number** |  | [optional] [default to undefined]
**name** | **string** |  | [default to undefined]
**isAdmin** | **boolean** |  | [optional] [default to undefined]
**isRenter** | **boolean** |  | [optional] [default to undefined]
**isLessor** | **boolean** |  | [optional] [default to undefined]
**cars** | [**Array&lt;Car&gt;**](Car.md) |  | [optional] [default to undefined]

## Example

```typescript
import { User } from './api';

const instance: User = {
    id,
    userName,
    normalizedUserName,
    email,
    normalizedEmail,
    emailConfirmed,
    passwordHash,
    securityStamp,
    concurrencyStamp,
    phoneNumber,
    phoneNumberConfirmed,
    twoFactorEnabled,
    lockoutEnd,
    lockoutEnabled,
    accessFailedCount,
    name,
    isAdmin,
    isRenter,
    isLessor,
    cars,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
