import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import type { RootDispatch } from "../Store";
import { apiExec } from "../../util/ApiUtils.ts";

interface State {
    value: string | undefined;
    valueNumber: number;
}

const reduceAddUser = (draft: Draft<State>, action: PayloadAction<string>) => {
    draft.value = action.payload;
    draft.valueNumber = 2;
};

const slice = createSlice({
    name: "Users",
    initialState: {
        value: undefined,
        valueNumber: 1,
    } as State,
    reducers: {
        USER: reduceAddUser,
    },
});

const addUser = slice.actions["USER"];

const fetchAllCars = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExec(ProjectControllerApi, (api) =>
            api.getAllProject()
        );
        if (hasFailed(response)) {
            dispatch(projectError(response.error));
        } else {
            dispatch(setProjects(response.data));
        }
    };
};

export { slice as UsersSlice, addUser, fetchUsers };
