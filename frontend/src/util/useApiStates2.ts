import { type RootState, useAppSelector } from "../store/Store";

type AllowedStoreNames = keyof RootState;

type UniqueStoreNames<K extends AllowedStoreNames> = K;

const useApiStates2 = <K extends AllowedStoreNames>(
    ...storeNames:
        | [UniqueStoreNames<K>, UniqueStoreNames<K>]
        | [UniqueStoreNames<K>]
): { [P in K]: RootState[P] } => {
    const selector = useAppSelector;

    const result = {} as { [P in K]: RootState[P] };

    storeNames.forEach((name) => {
        result[name] = selector(
            (state) => state[name]
        ) as RootState[typeof name];
    });

    return result;
};

export default useApiStates2;
