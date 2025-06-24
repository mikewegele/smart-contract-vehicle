import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import { type ApiError } from "../../util/ApiUtils.ts";

interface State {
    value: string[];
    error?: ApiError;
}

const reduceLogsError = (
    draft: Draft<State>,
    action: PayloadAction<ApiError>
) => {
    draft.error = action.payload;
};

const reduceAddLog = (draft: Draft<State>, action: PayloadAction<string>) => {
    draft.error = undefined;
    draft.value.push(action.payload);
};

const slice = createSlice({
    name: "Logs",
    initialState: {
        value: [],
    } as State,
    reducers: {
        ADD_LOG: reduceAddLog,
        SET_ERROR: reduceLogsError,
    },
});

const addLog = slice.actions["ADD_LOG"];
const reservationError = slice.actions["SET_ERROR"];

export { slice as LogSlice, addLog, reservationError };
