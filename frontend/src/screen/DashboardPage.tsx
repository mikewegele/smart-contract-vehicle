import React, { useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import SimpleMap from "../components/vehicle/VehicleMap.tsx";
import VehicleList from "../components/vehicle/VehicleList.tsx";
import useApiStates from "../util/useApiStates.ts";
import {
    fetchAllCars,
    fetchAllFuelTypes,
    fetchCarsByFilter,
} from "../store/reducer/cars.ts";
import { useAppDispatch } from "../store/Store.ts";
import { Box } from "@mui/material";
import { useGeolocation } from "../util/location/useGeolocation.ts";
import makeStyles from "../util/makeStyles.ts";
import VehicleFilterPanel from "../components/vehicle/VehicleFilterPanel.tsx";
import type { GeoSpatialQueryTO } from "../api";

const useStyles = makeStyles(() => ({
    mainContainer: {
        display: "flex",
        flexDirection: "row",
        justifyContent: "center",
        gap: "32px",
        marginTop: "16px",
        alignItems: "flex-start",
        padding: "2rem",
    },
    filterBox: {
        width: "300px",
        flexShrink: 0,
    },
    mapWrapper: {
        flex: 1,
        display: "flex",
        justifyContent: "center",
    },
    mapBox: {
        width: "100%",
    },
    rightBox: {
        width: "300px",
        flexShrink: 0,
    },
}));

const DashboardPage: React.FC = () => {
    const { classes } = useStyles();
    const dispatch = useAppDispatch();

    const [appliedFilters, setAppliedFilters] = useState<boolean>(false);

    useEffect(() => {
        dispatch(fetchAllCars());
        dispatch(fetchAllFuelTypes());
    }, [dispatch]);

    const { position } = useGeolocation();
    const { cars } = useApiStates("cars");

    const handleFilterApply = (filters: GeoSpatialQueryTO) => {
        dispatch(fetchCarsByFilter(filters, position));
        setAppliedFilters(true);
    };

    return (
        <Container>
            <NavLinks isLoggedIn={true} />

            <Box className={classes.mainContainer}>
                <Box className={classes.filterBox}>
                    <VehicleFilterPanel
                        onApply={handleFilterApply}
                        position={position}
                    />
                </Box>

                <Box className={classes.mapWrapper}>
                    <Box className={classes.mapBox}>
                        <SimpleMap vehicles={cars.value} />
                    </Box>
                </Box>

                <Box className={classes.rightBox}></Box>
            </Box>

            {appliedFilters && (
                <Box mt={4}>
                    <VehicleList vehicles={cars.value} />
                </Box>
            )}
        </Container>
    );
};

export default DashboardPage;
