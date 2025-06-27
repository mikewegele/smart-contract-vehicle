import { combineReducers, configureStore } from "@reduxjs/toolkit";
import { useDispatch, useSelector } from "react-redux";
import { CarsSlice } from "./reducer/cars.ts";
import { UserSlice } from "./reducer/user.ts";
import { ReservationSlice } from "./reducer/reservation.ts";
import { LogSlice } from "./reducer/logs.ts";

export const rootReducer = combineReducers({
    cars: CarsSlice.reducer,
    user: UserSlice.reducer,
    reservation: ReservationSlice.reducer,
    logs: LogSlice.reducer,
});

export type RootState = ReturnType<typeof rootReducer>;

export const ClearStore = {
    type: "Root:Clear",
};

export const store = configureStore({
    reducer: <RootState>(state: RootState, action: { type: string }) => {
        if (action === ClearStore) {
            return rootReducer(undefined, action);
        }

        // See https://github.com/rt2zz/redux-persist/issues/1140
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        return rootReducer(state as any, action);
    },
});

export type RootDispatch = typeof store.dispatch;

export const useAppDispatch = useDispatch.withTypes<RootDispatch>();
export const useAppSelector = useSelector.withTypes<RootState>();

export type ReducerReturn = {};
