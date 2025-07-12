import type { AxiosResponse } from "axios";
import { Configuration } from "../api";
import { BaseAPI } from "../api/base";

export interface ApiError {
    message: string;
    code: number;
}

interface FailedApiResponse {
    error: ApiError;
    data: undefined;
}

interface SuccessfulApiResponse<T> {
    data: T;
    error: undefined;
}

export type ApiResponse<T> = SuccessfulApiResponse<T> | FailedApiResponse;

export const hasFailed = <T>(
    response: ApiResponse<T>
): response is FailedApiResponse => {
    return response.error !== undefined;
};

export const apiExec = async <API extends BaseAPI, T>(
    API: new (config: Configuration) => API,
    execute: (api: API) => Promise<AxiosResponse<T>>
): Promise<ApiResponse<T>> => {
    try {
        const config = new Configuration();
        const response = await execute(new API(config));

        if (Math.floor(response.status / 100) === 2) {
            return {
                data: response.data,
                error: undefined,
            };
        } else {
            return {
                error: {
                    message: response.statusText,
                    code: response.status,
                },
                data: undefined,
            };
        }
    } catch (error: unknown) {
        if (error instanceof Error) {
            return {
                error: {
                    message: error.message,
                    code: 0,
                },
                data: undefined,
            };
        } else {
            return {
                error: {
                    message: "Unknown error occurred",
                    code: 0,
                },
                data: undefined,
            };
        }
    }
};

export const apiExecWithToken = async <API extends BaseAPI, T>(
    API: new (config: Configuration) => API,
    execute: (api: API) => Promise<AxiosResponse<T>>
): Promise<ApiResponse<T>> => {
    try {
        const token = localStorage.getItem("token");
        const config = new Configuration({
            baseOptions: {
                headers: {
                    Authorization: token ? `Bearer ${token}` : "",
                },
            },
        });
        const response = await execute(new API(config));

        if (Math.floor(response.status / 100) === 2) {
            return {
                data: response.data,
                error: undefined,
            };
        } else {
            return {
                error: {
                    message: response.statusText,
                    code: response.status,
                },
                data: undefined,
            };
        }
    } catch (error: unknown) {
        if (error instanceof Error) {
            return {
                error: {
                    message: error.message,
                    code: 0,
                },
                data: undefined,
            };
        } else {
            return {
                error: {
                    message: "Unknown error occurred",
                    code: 0,
                },
                data: undefined,
            };
        }
    }
};
