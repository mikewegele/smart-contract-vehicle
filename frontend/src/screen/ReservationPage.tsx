import React, { useCallback, useEffect, useMemo, useState } from "react";
import VehicleMap from "../components/vehicle/VehicleMap.tsx";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import Typography from "@mui/material/Typography";
import makeStyles from "../util/makeStyles.ts";
import DefaultButton from "../components/button/DefaultButton.tsx";
import { apiExec, hasFailed } from "../util/ApiUtils.ts";
import { BookingApi } from "../api";
import { useParams } from "react-router-dom";
import { useAppDispatch } from "../store/Store.ts";
import { fetchAllCars } from "../store/reducer/cars.ts";
import { fetchAllReservations } from "../store/reducer/reservation.ts";
import useApiStates from "../util/useApiStates.ts";

const useStyles = makeStyles(() => ({
    timerText: {
        marginTop: "16px",
        fontWeight: "bold",
        fontSize: "1.2rem",
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

    const { cars, user, reservation } = useApiStates(
        "cars",
        "user",
        "reservation"
    );

    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchAllCars());
        dispatch(fetchAllReservations());
    }, [dispatch]);

    const [expired, setExpired] = useState(false);

    const reservationCarObject = useMemo(() => {
        return reservation.value.find(
            (reservation) => reservation.reservedCarId === carId
        );
    }, [carId, reservation.value]);

    const initialTimeLeft = useMemo(() => {
        const blockingTimeUTC = reservationCarObject?.blockageTimeUTC;
        const blockageTime = blockingTimeUTC ? new Date(blockingTimeUTC) : null;
        const expiryTime = blockageTime
            ? new Date(blockageTime.getTime() + 15 * 60 * 1000)
            : null;
        const now = new Date();
        return expiryTime
            ? Math.max(
                  1,
                  Math.floor((expiryTime.getTime() - now.getTime()) / 1000)
              )
            : 1;
    }, [reservationCarObject?.blockageTimeUTC]);

    console.log(initialTimeLeft);

    const [timeLeft, setTimeLeft] = useState(initialTimeLeft);

    const reservedCar = useMemo(() => {
        return reservationCarObject
            ? cars.value.find((car) => car.carId === carId)
            : undefined;
    }, [carId, cars.value, reservationCarObject]);

    const handleCancel = useCallback(async () => {
        if (!reservedCar) return;
        // await apiExec(CarApi, (api) => api.apiCarCancelReservation(reservedCar.carId));
        setExpired(true);
    }, [reservedCar]);

    const handleUnlock = useCallback(async () => {
        const reservedCarId = reservedCar?.carId;
        const userId = user.value.id;
        if (!reservedCarId || !userId) {
            return;
        }
        const response = await apiExec(BookingApi, (api) =>
            api.apiBookingUnlockCarPost({
                reservedCarId: reservedCarId,
                rentorId: userId,
            })
        );
        if (hasFailed(response)) {
            // error
        } else {
            // drive
        }
    }, [reservedCar?.carId, user.value.id]);

    useEffect(() => {
        if (!reservedCar || expired) return;

        const interval = setInterval(() => {
            setTimeLeft((prev) => {
                if (prev <= 1) {
                    clearInterval(interval);
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(interval);
    }, [reservedCar, expired]);

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
            <Typography className={classes.timerText} variant="h6">
                Reservation expires in {minutes}:
                {seconds.toString().padStart(2, "0")}
            </Typography>
            <VehicleMap vehicles={[reservedCar]} />
            <DefaultButton
                onClick={handleCancel}
                variant="outlined"
                buttonClassName={classes.button}
            >
                Cancel reservation manually
            </DefaultButton>
            <DefaultButton
                onClick={handleUnlock}
                variant="outlined"
                buttonClassName={classes.button}
            >
                Drive Car
            </DefaultButton>
        </Container>
    );
};

export default ReservationPage;
