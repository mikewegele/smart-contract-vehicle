import { createSlice, type Draft, type PayloadAction } from "@reduxjs/toolkit";
import type { RootDispatch } from "../Store";
import {
    type ApiError,
    apiExecWithToken,
    hasFailed,
} from "../../util/ApiUtils.ts";
import { UserApi, type UserTO } from "../../api";

interface State {
    value: UserTO;
    error?: ApiError;
}

const reduceUserError = (
    draft: Draft<State>,
    action: PayloadAction<ApiError>
) => {
    draft.error = action.payload;
};

const reduceSetUser = (draft: Draft<State>, action: PayloadAction<UserTO>) => {
    draft.error = undefined;
    draft.value = action.payload;
};

const slice = createSlice({
    name: "User",
    initialState: {
        value: [],
    } as State,
    reducers: {
        SET_USER: reduceSetUser,
        SET_ERROR: reduceUserError,
    },
});

const addUser = slice.actions["SET_USER"];
const userError = slice.actions["SET_ERROR"];

const fetchUser = () => {
    return async (dispatch: RootDispatch): Promise<void> => {
        const response = await apiExecWithToken(UserApi, (api) =>
            api.apiUserProfileGet()
        );
        if (hasFailed(response)) {
            dispatch(userError(response.error));
        } else {
            dispatch(addUser(response.data));
        }
    };
};

export { slice as UserSlice, addUser, userError, fetchUser };
