import React, { useEffect } from "react";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import SimpleMap from "../components/vehicle/VehicleMap.tsx";
import VehicleList from "../components/vehicle/VehicleList.tsx";
import useApiStates from "../util/useApiStates.ts";
import { fetchAllCars } from "../store/reducer/cars.ts";
import { useAppDispatch } from "../store/Store.ts";

export interface VehicleProp {
    image: string;
    model: string;
    pricePerMinute: number;
    seats: number;
    rangeKm: number;
    location: [number, number];
}

const DashboardPage: React.FC = () => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchAllCars());
    }, [dispatch]);

    const { cars } = useApiStates("cars");

    return (
        <Container>
            <NavLinks isLoggedIn={true} />
            <SimpleMap vehicles={cars.value} />
            <VehicleList vehicles={cars.value} />
        </Container>
    );
};

export default DashboardPage;
