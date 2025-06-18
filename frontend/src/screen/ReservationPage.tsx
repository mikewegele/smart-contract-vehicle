import React, { useCallback, useEffect, useState } from "react";
import useApiStates from "../util/useApiStates.ts";
import VehicleMap from "../components/vehicle/VehicleMap.tsx";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import Typography from "@mui/material/Typography";
import makeStyles from "../util/makeStyles.ts";
import DefaultButton from "../components/button/DefaultButton.tsx";
import { apiExec, hasFailed } from "../util/ApiUtils.ts";
import { BookingApi } from "../api";

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

    const cars = useApiStates("cars");
    const user = useApiStates("user");

    const [timeLeft, setTimeLeft] = useState(15 * 60);
    const [expired, setExpired] = useState(false);

    const reservedCar = cars.cars.reservedCar;

    const handleCancel = async () => {
        if (!reservedCar) return;
        // await apiExec(CarApi, (api) => api.apiCarCancelReservation(reservedCar.carId));
        setExpired(true);
    };

    const handleUnlock = useCallback(async () => {
        const reservedCarId = cars.cars.reservedCar?.carId;
        const userId = user.user.value.id;
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
    }, [cars.cars.reservedCar?.carId, user.user.value.id]);

    useEffect(() => {
        if (!reservedCar || expired) return;

        const interval = setInterval(() => {
            setTimeLeft((prev) => {
                if (prev <= 1) {
                    clearInterval(interval);
                    handleCancel();
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(interval);
    }, [reservedCar, expired, handleCancel]);

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
                Unlock Car
            </DefaultButton>
        </Container>
    );
};

export default ReservationPage;
