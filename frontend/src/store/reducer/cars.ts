import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import type { RootDispatch } from "../Store";
import { type ApiError, apiExec, hasFailed } from "../../util/ApiUtils.ts";
import { CarApi, type CarTO, type GeoSpatialQueryTO } from "../../api";
import type { Position } from "../../util/location/useGeolocation.ts";

interface State {
    value: CarTO[];
    maxSeats: number;
    maxPricePerMinute: number;
    fuelTypes: string[];
    driveTrains: string[];
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

    const seatCounts = action.payload.map((car) => car.seats);
    draft.maxSeats =
        seatCounts.length > 0 ? Math.max(...seatCounts) : draft.maxSeats;

    const pricePerMinuteCounts = action.payload.map(
        (car) => car.pricePerMinute
    );
    draft.maxPricePerMinute =
        pricePerMinuteCounts.length > 0
            ? Math.max(...pricePerMinuteCounts)
            : draft.maxPricePerMinute;
};

const reduceSetFuelTypes = (
    draft: Draft<State>,
    action: PayloadAction<string[]>
) => {
    draft.error = undefined;
    draft.fuelTypes = action.payload;
};

const reduceSetDriveTrains = (
    draft: Draft<State>,
    action: PayloadAction<string[]>
) => {
    draft.error = undefined;
    draft.driveTrains = action.payload;
};

const slice = createSlice({
    name: "Cars",
    initialState: {
        value: [],
        fuelTypes: [],
        driveTrains: [],
    } as State,
    reducers: {
        SET_CARS: reduceSetCars,
        SET_ERROR: reduceCarError,
        SET_FUEL_TYPES: reduceSetFuelTypes,
        ADD_DRIVE_TRAINS: reduceSetDriveTrains,
    },
});

const addCars = slice.actions["SET_CARS"];
const addFuelTypes = slice.actions["SET_FUEL_TYPES"];
const addAllDriveTrains = slice.actions["ADD_DRIVE_TRAINS"];
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

const fetchAllFuelTypes = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarGetFueltypesGet()
        );
        if (hasFailed(response)) {
            dispatch(carError(response.error));
        } else {
            dispatch(addFuelTypes(response.data));
        }
    };
};

const fetchAllDriveTrains = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarGetDrivetrainsGet()
        );
        if (hasFailed(response)) {
            dispatch(carError(response.error));
        } else {
            dispatch(addAllDriveTrains(response.data));
        }
    };
};

const fetchCarsByFilter = (
    filters: GeoSpatialQueryTO,
    position: Position | null
) => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarGeoSpatialQueryPost({
                userLocation: {
                    type: "Point",
                    coordinates: [position?.longitude, position?.latitude],
                },
                maxDistance: filters.maxDistance || 10,
                minSeats: filters.minSeats,
                maxSeats: filters.maxSeats,
                minPricePerMinute: filters.minPricePerMinute,
                maxPricePerMinute: filters.maxPricePerMinute,
                allowedDrivetrains: filters.allowedDrivetrains,
            })
        );
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
    fetchAllFuelTypes,
    fetchAllDriveTrains,
};
