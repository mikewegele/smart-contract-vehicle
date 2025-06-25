import React, { useCallback, useEffect, useMemo, useState } from "react";
import VehicleMap from "../components/vehicle/VehicleMap.tsx";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import Typography from "@mui/material/Typography";
import DefaultButton from "../components/button/DefaultButton.tsx";
import { apiExecWithToken, hasFailed } from "../util/ApiUtils.ts";
import { BookingApi } from "../api";
import { useNavigate, useParams } from "react-router-dom";
import { useAppDispatch } from "../store/Store.ts";
import { fetchAllCars } from "../store/reducer/cars.ts";
import { fetchAllReservations } from "../store/reducer/reservation.ts";
import useApiStates from "../util/useApiStates.ts";
import { Box } from "@mui/material";
import { makeStyles } from "tss-react/mui";
import { useWeb3 } from "../web3/Web3Provider.tsx";
import { addLog } from "../store/reducer/logs.ts";
import FeedbackSnackbar from "../components/snackbar/FeedbackSnackbar.tsx";

const useStyles = makeStyles()(() => ({
    box: {
        width: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "16px",
        marginTop: "16px",
    },
    timerText: {
        marginBottom: "16px",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        fontWeight: "bold",
        fontSize: "1.2rem",
        background: "rgba(255, 255, 255, 0.75)",
        borderRadius: "12px",
        padding: "16px 24px",
        boxShadow: "0 8px 32px 0 rgba(0, 0, 0, 0.1)",
        backdropFilter: "blur(8px)",
        border: "1px solid rgba(255, 255, 255, 0.3)",
        color: "#34495e",
        textAlign: "center",
    },
    button: {
        width: "400px",
    },
    expiredMessage: {
        color: "#d32f2f",
        fontWeight: "bold",
        marginTop: "20px",
    },
}));

const ReservationPage: React.FC = () => {
    const { classes } = useStyles();

    const { carId } = useParams();

    const web3 = useWeb3();

    const { cars, user, reservation } = useApiStates(
        "cars",
        "user",
        "reservation"
    );

    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");

    const navigate = useNavigate();

    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchAllCars());
        dispatch(fetchAllReservations());
    }, [dispatch]);

    const reservationCarObject = useMemo(() => {
        return reservation.value.find(
            (reservation) => reservation.reservedCarId === carId
        );
    }, [carId, reservation.value]);

    const RESERVATION_TIMEOUT_MINUTES = 15;

    const [timeLeft, setTimeLeft] = useState<number | null>(null);
    const [expired, setExpired] = useState(false);

    useEffect(() => {
        if (!reservationCarObject?.blockageTimeUTC) return;

        const blockageTime = new Date(reservationCarObject.blockageTimeUTC);
        const now = new Date();

        const elapsedMs = now.getTime() - blockageTime.getTime();
        const elapsedSeconds = Math.floor(elapsedMs / 1000);
        const totalTimeoutSeconds = RESERVATION_TIMEOUT_MINUTES * 60;
        const remaining = totalTimeoutSeconds - elapsedSeconds;

        const initialTimeLeft = Math.max(remaining, 0);
        setTimeLeft(initialTimeLeft);

        if (initialTimeLeft === 0) {
            setExpired(true);
        }
    }, [reservationCarObject?.blockageTimeUTC]);

    useEffect(() => {
        if (timeLeft === null || timeLeft <= 0) return;

        const interval = setInterval(() => {
            setTimeLeft((prev) => {
                if (prev === null || prev <= 1) {
                    clearInterval(interval);
                    setExpired(true);
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(interval);
    }, [timeLeft]);

    const reservedCar = useMemo(() => {
        return reservationCarObject
            ? cars.value.find((car) => car.carId === carId)
            : undefined;
    }, [carId, cars.value, reservationCarObject]);

    const handleCancel = useCallback(async () => {
        if (!reservedCar) {
            return;
        }
        if (
            !web3.web3 ||
            !web3.account ||
            !web3.contract ||
            !reservedCar ||
            !user.value.id ||
            !reservedCar.carId
        ) {
            return null;
        }
        try {
            const receipt = await web3.contract.methods
                .cancelReservation(reservedCar.carId, user.value.id)
                .send({
                    from: web3.account,
                });
            dispatch(
                addLog({
                    name: "Cancel Reservation in Blockchain",
                    message: `Cancelled Car: ${reservedCar.carId}`,
                    id: receipt.transactionHash,
                })
            );
        } catch {
            return null;
        }
        setExpired(true);
    }, [
        dispatch,
        reservedCar,
        user.value.id,
        web3.account,
        web3.contract,
        web3.web3,
    ]);

    const handleDrive = useCallback(async (): Promise<true | null> => {
        if (
            !reservedCar ||
            !web3.web3 ||
            !web3.account ||
            !web3.contract ||
            !user.value.id ||
            !reservedCar.carId
        ) {
            return null;
        }

        try {
            const receipt = await web3.contract.methods
                .driveCar(reservedCar.carId, user.value.id)
                .send({
                    from: web3.account,
                });

            dispatch(
                addLog({
                    name: "Drive Car on Blockchain",
                    message: `Driving Car: ${reservedCar.carId}`,
                    id: receipt.transactionHash,
                })
            );
            return true;
        } catch {
            return null;
        }
    }, [
        dispatch,
        reservedCar,
        user.value.id,
        web3.account,
        web3.contract,
        web3.web3,
    ]);

    const handleUnlock = useCallback(async () => {
        const reservedCarId = reservedCar?.carId;
        const userId = user.value.id;
        if (!reservedCarId || !userId) {
            return;
        }
        const response = await apiExecWithToken(BookingApi, (api) =>
            api.apiBookingUnlockCarPost({
                reservedCarId: reservedCarId,
                rentorId: userId,
                id: reservationCarObject?.id,
            })
        );
        if (hasFailed(response)) {
            setFeedbackSeverity("success");
            setFeedbackMsg("Failed to unlock the car");
        } else {
            const driveResult = await handleDrive();
            console.log(driveResult);
            if (!driveResult) {
                setFeedbackSeverity("error");
                setFeedbackMsg("Failed to drive the car on chain");
                return;
            }
            navigate(`/driving/${reservedCarId}`);
        }
    }, [navigate, reservedCar?.carId, user.value.id]);

    if (!reservedCar || expired) {
        return (
            <Container>
                <NavLinks isLoggedIn={true} />
                <Typography className={classes.expiredMessage} variant="h6">
                    Reservation was canceled or has expired.
                </Typography>
            </Container>
        );
    }

    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;

    return (
        <Container>
            <NavLinks isLoggedIn={true} />
            <Box className={classes.box}>
                <Typography className={classes.timerText} variant="h6">
                    Reservation expires in {minutes}:
                    {seconds.toString().padStart(2, "0")}
                </Typography>
            </Box>
            <VehicleMap vehicles={[reservedCar]} />
            <Box className={classes.box}>
                <DefaultButton
                    onClick={handleCancel}
                    variant="outlined"
                    buttonclassname={classes.button}
                >
                    Cancel reservation manually
                </DefaultButton>
                <DefaultButton
                    onClick={handleUnlock}
                    variant="outlined"
                    buttonclassname={classes.button}
                >
                    Drive Car
                </DefaultButton>
            </Box>
            <FeedbackSnackbar
                open={feedbackOpen}
                message={feedbackMsg}
                severity={feedbackSeverity}
                onClose={() => setFeedbackOpen(false)}
            />
        </Container>
    );
};

export default ReservationPage;
