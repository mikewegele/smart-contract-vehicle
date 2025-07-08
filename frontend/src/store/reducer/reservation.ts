import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import type { RootDispatch } from "../Store";
import {
    type ApiError,
    apiExecWithToken,
    hasFailed,
} from "../../util/ApiUtils.ts";
import { BookingApi, type ReservationTO } from "../../api";

interface State {
    value: ReservationTO[];
    error?: ApiError;
}

const reduceReservationError = (
    draft: Draft<State>,
    action: PayloadAction<ApiError>
) => {
    draft.error = action.payload;
};

const reduceSetReservations = (
    draft: Draft<State>,
    action: PayloadAction<ReservationTO[]>
) => {
    draft.error = undefined;
    draft.value = action.payload;
};

const slice = createSlice({
    name: "Cars",
    initialState: {
        value: [],
        fuelTypes: [],
        driveTrains: [],
        maxSeats: 0,
        maxPricePerMinute: 0,
    } as State,
    reducers: {
        SET_RESERVATIONS: reduceSetReservations,
        SET_ERROR: reduceReservationError,
    },
});

const addReservations = slice.actions["SET_RESERVATIONS"];
const reservationError = slice.actions["SET_ERROR"];

const fetchAllReservations = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExecWithToken(BookingApi, (api) =>
            api.apiBookingGetAllReservationsGet()
        );
        if (hasFailed(response)) {
            dispatch(reservationError(response.error));
        } else {
            dispatch(addReservations(response.data));
        }
    };
};

export { slice as ReservationSlice, fetchAllReservations };
