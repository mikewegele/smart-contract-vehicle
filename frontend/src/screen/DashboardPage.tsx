import React, { useCallback, useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import SimpleMap from "../components/vehicle/VehicleMap.tsx";
import VehicleList from "../components/vehicle/VehicleList.tsx";
import useApiStates from "../util/useApiStates.ts";
import {
    fetchAllCars,
    fetchAllDriveTrains,
    fetchAllFuelTypes,
    fetchCarsByFilter,
} from "../store/reducer/cars.ts";
import {
    fetchUser,
} from "../store/reducer/user.ts";
import { useAppDispatch } from "../store/Store.ts";
import { Box } from "@mui/material";
import { useGeolocation } from "../util/location/useGeolocation.ts";
import VehicleFilterPanel from "../components/vehicle/VehicleFilterPanel.tsx";
import { BookingApi, type CarTO, type GeoSpatialQueryTO } from "../api";
import { makeStyles } from "tss-react/mui";
import { apiExecWithToken, hasFailed } from "../util/ApiUtils.ts";
import { useWeb3 } from "../web3/Web3Provider.tsx";
import { useNavigate } from "react-router-dom";
import ReservationDialog from "../components/vehicle/reservation/ReservationDialog.tsx";
import { addLog } from "../store/reducer/logs.ts";
import FeedbackSnackbar from "../components/snackbar/FeedbackSnackbar.tsx";
import { v4 as uuidv4 } from "uuid";

const useStyles = makeStyles()(() => ({
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
    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");
    const [openDialog, setOpenDialog] = useState(false);
    const [currentVehicle, setCurrentVehicle] = useState<CarTO | null>(null);

    const handleOpen = useCallback((vehicle: CarTO) => {
        setOpenDialog(true);
        setCurrentVehicle(vehicle);
    }, []);

    const handleClose = () => setOpenDialog(false);

    const web3Context = useWeb3();

    useEffect(() => {
        dispatch(fetchUser())
        dispatch(fetchAllCars());
        dispatch(fetchAllFuelTypes());
        dispatch(fetchAllDriveTrains());
    }, [dispatch]);

    const { position } = useGeolocation();
    const { cars, user } = useApiStates("cars", "user");

    const navigate = useNavigate();

    const handleFilterApply = (filters: GeoSpatialQueryTO) => {
        dispatch(fetchCarsByFilter(filters, position));
        setAppliedFilters(true);
    };

    const reservationHasFailed = useCallback(() => {
        setFeedbackMsg("Failed to reserve car.");
        setFeedbackSeverity("error");
        setFeedbackOpen(true);
    }, []);

    const blockCar = async (carId: string) => {
        return await apiExecWithToken(BookingApi, (api) =>
            api.apiBookingBlockCarPost(carId)
        );
    };

    const rentCarOnChain = async (vehicle: CarTO, userId: string) => {
        if (
            !web3Context.web3 ||
            !web3Context.account ||
            !web3Context.contract ||
            !vehicle.pricePerMinute
        ) {
            return null;
        }

        try {
            return await web3Context.contract.methods
                .rentCar(vehicle.carId, userId)
                .send({
                    from: web3Context.account,
                    value: web3Context.web3.utils.toWei("0.01", "ether"),
                });
        } catch {
            return null;
        }
    };

    const reserveCarInBackend = async (
        carId: string,
        userId: string,
        txHash: string,
        reservationId: string
    ) => {
        const response = await apiExecWithToken(BookingApi, (api) =>
            api.apiBookingReserveCarPost({
                reservedCarId: carId,
                rentorId: userId,
                blockchainTransactionId: txHash,
                id: reservationId,
            })
        );
        if (!hasFailed(response)) {
            dispatch(
                addLog({
                    logId: uuidv4(),
                    name: "Reservation Confirmed from Backend",
                    message: response.data,
                    id: txHash,
                    timestamp: new Date().getTime(),
                })
            );
        }
        return !hasFailed(response);
    };

    const clickOnConfirm = useCallback(
        async (vehicle: CarTO) => {
            const userId = user.value.id;
            if (!userId) {
                return reservationHasFailed();
            }
            const blockResponse = await blockCar(vehicle.carId);
            if (hasFailed(blockResponse)) {
                return reservationHasFailed();
            }
            const receipt = await rentCarOnChain(vehicle, userId);
            if (!receipt) {
                return reservationHasFailed();
            }
            const reservationId = blockResponse.data.id;
            const event = receipt.events?.CarRented?.returnValues;
            const etherValue = web3Context.web3?.utils.fromWei(
                event.value.toString(),
                "ether"
            );
            dispatch(
                addLog({
                    logId: uuidv4(),
                    name: "Reserve Car",
                    message: `Reserve car: ${reservationId} in Blockchain`,
                    id: receipt.transactionHash,
                    timestamp: new Date().getTime(),
                    value: etherValue,
                })
            );
            if (!reservationId) {
                return reservationHasFailed();
            }
            const reserveSuccess = await reserveCarInBackend(
                vehicle.carId,
                userId,
                receipt.transactionHash,
                reservationId
            );
            if (!reserveSuccess) {
                return reservationHasFailed();
            }
            navigate(`/reservation/${vehicle.carId}`);
        },
        [navigate, rentCarOnChain, reservationHasFailed, user.value.id]
    );

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
                        <SimpleMap
                            vehicles={cars.value}
                            openDialog={handleOpen}
                        />
                    </Box>
                </Box>

            </Box>
            {appliedFilters && (
                <Box mt={4}>
                    <VehicleList
                        vehicles={cars.value}
                        clickOnConfirm={clickOnConfirm}
                        handleOpen={handleOpen}
                    />
                </Box>
            )}
            {currentVehicle && (
                <ReservationDialog
                    open={openDialog}
                    onClose={handleClose}
                    onConfirm={() => clickOnConfirm(currentVehicle)}
                    car={currentVehicle}
                />
            )}
            <FeedbackSnackbar
                open={feedbackOpen}
                message={feedbackMsg}
                severity={feedbackSeverity}
                onClose={() => setFeedbackOpen(false)}
            />
        </Container>
    );
};

export default DashboardPage;
