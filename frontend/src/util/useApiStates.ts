import { type RootState, useAppSelector } from "../store/Store";

type AllowedStoreNames = keyof RootState;

type NoDuplicates<
    T extends readonly any[],
    Seen extends any[] = [],
> = T extends [infer First, ...infer Rest]
    ? First extends Seen[number]
        ? never
        : NoDuplicates<Rest, [...Seen, First]>
    : T;

const useApiStates = <K extends AllowedStoreNames, T extends readonly K[]>(
    ...storeNames: NoDuplicates<T> extends never ? never : T
): { [P in T[number]]: RootState[P] } => {
    const selector = useAppSelector;
    const result = {} as { [P in T[number]]: RootState[P] };

    storeNames.forEach((name) => {
        result[name] = selector(
            (state) => state[name]
        ) as RootState[typeof name];
    });

    return result;
};

export default useApiStates;
