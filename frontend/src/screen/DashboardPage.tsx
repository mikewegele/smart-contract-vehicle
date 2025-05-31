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

const vehicles: VehicleProp[] = [
    {
        image: "https://cdn.sanity.io/images/767s1cf5/production/81923a4526c2622bfd3463f7942c022472c868fc-1000x714.png?rect=0,94,1000,525&w=1200&h=630&fm=png",
        model: "Fiat 500e",
        pricePerMinute: 0.25,
        seats: 4,
        rangeKm: 230,
        location: [52.52, 13.405],
    },
    {
        image: "https://www.fett-wirtz.de//assets/components/phpthumbof/cache/i3-rendering.a175f4b33a701463542158cc33d89ecf.webp",
        model: "BMW i3",
        pricePerMinute: 0.35,
        seats: 4,
        rangeKm: 250,
        location: [52.515, 13.39],
    },
];

const DashboardPage: React.FC = () => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchAllCars());
    }, [dispatch]);

    const { cars } = useApiStates("cars");

    console.log(cars);

    return (
        <Container>
            <NavLinks isLoggedIn={true} />
            <SimpleMap vehicles={vehicles} />
            <VehicleList vehicles={vehicles} />
        </Container>
    );
};

export default DashboardPage;
