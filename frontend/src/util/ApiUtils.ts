import {getSmartContractVehicle} from "../api";

type ApiType = ReturnType<typeof getSmartContractVehicle>;

export function apiExec<T>(fn: (api: ApiType) => Promise<T>): Promise<T> {
    const api = getSmartContractVehicle();
    return fn(api);
}

export function hasFailed(status: number): boolean {
    return status.status < 200 || status.status >= 300;
}