/**
 * Generated by orval v7.9.0 🍺
 * Do not edit manually.
 * SmartContractVehicle
 * OpenAPI spec version: 1.0
 */
import { api } from './axios-config';
export interface Address {
  id?: number;
  /**
   * @minLength 0
   * @maxLength 10
   * @nullable
   */
  extraInfo?: string | null;
  /**
   * @minLength 0
   * @maxLength 50
   * @nullable
   */
  floor?: string | null;
  houseNumber: number;
  /**
   * @minLength 0
   * @maxLength 50
   */
  street: string;
  /**
   * @minLength 0
   * @maxLength 50
   */
  city: string;
  /**
   * @minLength 0
   * @maxLength 15
   */
  postalCode: string;
  /**
   * @minLength 0
   * @maxLength 110
   */
  country: string;
}

export interface User {
  id?: number;
  /** @minLength 1 */
  lastName: string;
  /** @minLength 1 */
  firstName: string;
  /** @minLength 1 */
  email: string;
  birthday: string;
  billing: Address;
  mailing: Address;
  /** @minLength 1 */
  password: string;
  /** @nullable */
  phoneNumber?: string | null;
  /** @minLength 1 */
  walletId: string;
}

export type GetParams = {
id?: number;
};

export type DeleteApiAddressDeleteParams = {
id?: number;
};

export type GetApiUserGetParams = {
id?: number;
};

export type DeleteApiUserDeleteParams = {
id?: number;
};

export const getSmartContractVehicle = () => {
const get = (
    params?: GetParams,
 ) => {
      return api<void>(
      {url: `/api/Address/Get`, method: 'GET',
        params
    },
      );
    }
  
const deleteApiAddressDelete = (
    params?: DeleteApiAddressDeleteParams,
 ) => {
      return api<void>(
      {url: `/api/Address/Delete`, method: 'DELETE',
        params
    },
      );
    }
  
const postApiAddressPost = (
    address: Address,
 ) => {
      return api<void>(
      {url: `/api/Address/Post`, method: 'POST',
      headers: {'Content-Type': 'application/json', },
      data: address
    },
      );
    }
  
const patchApiAddressUpdate = (
    address: Address,
 ) => {
      return api<void>(
      {url: `/api/Address/Update`, method: 'PATCH',
      headers: {'Content-Type': 'application/json', },
      data: address
    },
      );
    }
  
const getApiUserGet = (
    params?: GetApiUserGetParams,
 ) => {
      return api<void>(
      {url: `/api/User/Get`, method: 'GET',
        params
    },
      );
    }
  
const deleteApiUserDelete = (
    params?: DeleteApiUserDeleteParams,
 ) => {
      return api<void>(
      {url: `/api/User/Delete`, method: 'DELETE',
        params
    },
      );
    }
  
const postApiUserRegister = (
    user: User,
 ) => {
      return api<void>(
      {url: `/api/User/Register`, method: 'POST',
      headers: {'Content-Type': 'application/json', },
      data: user
    },
      );
    }
  
const patchApiUserUpdate = (
    user: User,
 ) => {
      return api<void>(
      {url: `/api/User/Update`, method: 'PATCH',
      headers: {'Content-Type': 'application/json', },
      data: user
    },
      );
    }
  
return {get,deleteApiAddressDelete,postApiAddressPost,patchApiAddressUpdate,getApiUserGet,deleteApiUserDelete,postApiUserRegister,patchApiUserUpdate}};
export type GetResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['get']>>>
export type DeleteApiAddressDeleteResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['deleteApiAddressDelete']>>>
export type PostApiAddressPostResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['postApiAddressPost']>>>
export type PatchApiAddressUpdateResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['patchApiAddressUpdate']>>>
export type GetApiUserGetResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['getApiUserGet']>>>
export type DeleteApiUserDeleteResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['deleteApiUserDelete']>>>
export type PostApiUserRegisterResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['postApiUserRegister']>>>
export type PatchApiUserUpdateResult = NonNullable<Awaited<ReturnType<ReturnType<typeof getSmartContractVehicle>['patchApiUserUpdate']>>>
