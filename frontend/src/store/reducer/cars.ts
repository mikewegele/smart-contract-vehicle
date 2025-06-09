import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import type { RootDispatch } from "../Store";
import { type ApiError, apiExec, hasFailed } from "../../util/ApiUtils.ts";
import { CarApi, type CarTO } from "../../api";
import type { FilterValues } from "../../components/vehicle/VehicleFilterPanel.tsx";
import type { Position } from "../../util/location/useGeolocation.ts";

interface State {
    value: CarTO[];
    error?: ApiError;
}

const reduceCarError = (
    draft: Draft<State>,
    action: PayloadAction<ApiError>
) => {
    draft.error = action.payload;
};

const reduceSetCars = (draft: Draft<State>, action: PayloadAction<CarTO[]>) => {
    draft.error = undefined;
    draft.value = action.payload;
};

const slice = createSlice({
    name: "Cars",
    initialState: {
        value: [],
    } as State,
    reducers: {
        SET_CARS: reduceSetCars,
        SET_ERROR: reduceCarError,
    },
});

const addCars = slice.actions["SET_CARS"];
const carError = slice.actions["SET_ERROR"];

const fetchAllCars = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarGetAllCarsGet()
        );
        if (hasFailed(response)) {
            dispatch(carError(response.error));
        } else {
            dispatch(addCars(response.data));
        }
    };
};

const fetchCarsByFilter = (
    filters: FilterValues,
    position: Position | null
) => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarGeoSpatialQueryPost({
                userLocation: {
                    type: "Point",
                    coordinates: position,
                },
                maxDistance: 10000,
                minSeats: filters.minSeats,
                maxSeats: filters.maxSeats,
                minPricePerMinute: filters.minPrice,
                maxPricePerMinute: filters.maxPrice,
                allowedDrivetrains: filters.drivetrain
                    ? [filters.drivetrain]
                    : undefined,
            })
        );
        console.log(response);
        if (hasFailed(response)) {
            dispatch(carError(response.error));
        } else {
            dispatch(addCars(response.data));
        }
    };
};

export {
    slice as CarsSlice,
    addCars,
    carError,
    fetchAllCars,
    fetchCarsByFilter,
};
