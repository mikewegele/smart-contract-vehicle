import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import { type ApiError } from "../../util/ApiUtils.ts";

export interface Log {
    logId: string;
    id?: string;
    name?: string;
    message?: string;
    timestamp?: number;
    value?: number;
}

interface State {
    value: Log[];
    error?: ApiError;
}

const reduceLogsError = (
    draft: Draft<State>,
    action: PayloadAction<ApiError>
) => {
    draft.error = action.payload;
};

const reduceAddLog = (draft: Draft<State>, action: PayloadAction<Log>) => {
    draft.error = undefined;
    draft.value.push(action.payload);
};

const reduceDeleteLog = (
    draft: Draft<State>,
    action: PayloadAction<string>
) => {
    draft.value = draft.value.filter((log) => log.logId !== action.payload);
};

const slice = createSlice({
    name: "Logs",
    initialState: {
        value: [],
    } as State,
    reducers: {
        ADD_LOG: reduceAddLog,
        SET_ERROR: reduceLogsError,
        DELETE_LOG: reduceDeleteLog,
    },
});

const addLog = slice.actions["ADD_LOG"];
const deleteLog = slice.actions["DELETE_LOG"];
const reservationError = slice.actions["SET_ERROR"];

export { slice as LogSlice, addLog, reservationError, deleteLog };
