import React, { useEffect } from "react";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import SimpleMap from "../components/vehicle/VehicleMap.tsx";
import VehicleList from "../components/vehicle/VehicleList.tsx";
import useApiStates from "../util/useApiStates.ts";
import { fetchAllCars, fetchCarsByFilter } from "../store/reducer/cars.ts";
import { useAppDispatch } from "../store/Store.ts";
import VehicleFilterPanel, {
    type FilterValues,
} from "../components/vehicle/VehicleFilterPanel.tsx";
import { Box } from "@mui/material";

const DashboardPage: React.FC = () => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchAllCars());
    }, [dispatch]);

    const handleFilterApply = (filters: FilterValues) => {
        dispatch(fetchCarsByFilter(filters));
    };

    const { cars } = useApiStates("cars");

    return (
        <Container>
            <NavLinks isLoggedIn={true} />
            <SimpleMap vehicles={cars.value} />
            <Box width="300px">
                <VehicleFilterPanel onApply={handleFilterApply} />
            </Box>
            <VehicleList vehicles={cars.value} />
        </Container>
    );
};

export default DashboardPage;
