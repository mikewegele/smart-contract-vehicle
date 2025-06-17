import React, { useEffect, useState } from "react";
import useApiStates from "../util/useApiStates.ts";
import VehicleMap from "../components/vehicle/VehicleMap.tsx";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import Typography from "@mui/material/Typography";
import makeStyles from "../util/makeStyles.ts";
import DefaultButton from "../components/button/DefaultButton.tsx";

const useStyles = makeStyles(() => ({
    timerText: {
        margin: "16px 0",
        fontWeight: "bold",
        fontSize: "1.2rem",
    },
    button: {
        width: "400px",
    },
    expiredMessage: {
        color: "#d32f2f",
        fontWeight: "bold",
        marginTop: 20,
    },
}));

const ReservationPage: React.FC = () => {
    const { classes } = useStyles();
    const cars = useApiStates("cars");
    const [timeLeft, setTimeLeft] = useState(15 * 60);
    const [expired, setExpired] = useState(false);

    const reservedCar = cars.cars.reservedCar;

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
    }, [reservedCar, expired]);

    const handleCancel = async () => {
        if (!reservedCar) return;
        // await apiExec(CarApi, (api) => api.apiCarCancelReservation(reservedCar.carId));
        setExpired(true);
    };

    if (!reservedCar || expired) {
        return (
            <Container>
                <NavLinks isLoggedIn={true} />
                <Typography className={classes.expiredMessage} variant="h6">
                    Reservierung wurde abgebrochen oder ist abgelaufen.
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
                Reservierung läuft ab in {minutes}:
                {seconds.toString().padStart(2, "0")}
            </Typography>
            <VehicleMap vehicles={[reservedCar]} />
            <DefaultButton
                onClick={handleCancel}
                variant="outlined"
                buttonClassName={classes.button}
            >
                Reservierung manuell abbrechen
            </DefaultButton>
        </Container>
    );
};

export default ReservationPage;
