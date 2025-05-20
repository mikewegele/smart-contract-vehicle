import { getSmartContractVehicle } from "../api";

type ApiType = ReturnType<typeof getSmartContractVehicle>;

export function apiExec<T>(fn: (api: ApiType) => Promise<T>): Promise<T> {
    const api = getSmartContractVehicle();
    return fn(api);
}

export function hasFailed(response: { status: number }): boolean {
    return response.status < 200 || response.status >= 300;
}