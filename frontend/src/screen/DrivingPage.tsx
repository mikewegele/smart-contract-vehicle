import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Grid, LinearProgress, Typography } from "@mui/material";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import { makeStyles } from "tss-react/mui";
import { useNavigate, useParams } from "react-router-dom";
import useApiStates from "../util/useApiStates.ts";
import { useAppDispatch } from "../store/Store.ts";
import { fetchAllCars } from "../store/reducer/cars.ts";
import DefaultButton from "../components/button/DefaultButton.tsx";
import { useWeb3 } from "../web3/Web3Provider.tsx";
import { addLog } from "../store/reducer/logs.ts";
import { v4 as uuidv4 } from "uuid";
import FeedbackSnackbar from "../components/snackbar/FeedbackSnackbar.tsx";
import { apiExecWithToken, hasFailed } from "../util/ApiUtils.ts";
import { BookingApi } from "../api";

const useStyles = makeStyles()(() => ({
    glassBox: {
        maxWidth: "60%",
        margin: "12px auto",
        borderRadius: "16px",
        padding: "16px 24px",
        background: "rgba(255, 255, 255, 0.1)",
        backdropFilter: "blur(10px)",
        WebkitBackdropFilter: "blur(10px)",
        border: "1px solid rgba(255, 255, 255, 0.3)",
        boxShadow: "0 8px 32px 0 rgba(0, 0, 0, 0.2)",
    },
    video: {
        width: "100%",
        borderRadius: "12px",
        display: "block",
    },
    label: {
        fontWeight: "bold",
        color: "#eee",
    },
    value: {
        fontSize: "1.5rem",
        fontWeight: 600,
        color: "#fff",
    },
    buttonBox: {
        display: "flex",
        justifyContent: "center",
        margin: "32px",
    },
    button: {
        width: "30%",
    },
}));

const stopTimes = [0.3, 5, 10.5, 14, 19.5, 24, 26, 29, 30.5, 35, 38];

const DrivingPage: React.FC = () => {
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
    }, [dispatch]);

    const web3 = useWeb3();

    const car = cars.value.find((car) => car.carId === carId);

    const navigate = useNavigate();

    const [speed, setSpeed] = useState(0);
    const [battery, setBattery] = useState(78);
    const [remainingReach, setRemainingReach] = useState<number | undefined>(
        undefined
    );
    const [temperature, setTemperature] = useState(27);
    const [currentTime, setCurrentTime] = useState(0);
    const [lastMoveTime, setLastMoveTime] = useState<number | null>(null);
    const maxSpeed = 30;
    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");

    const reservationCarObject = useMemo(() => {
        return reservation.value.find(
            (reservation) => reservation.reservedCarId === carId
        );
    }, [carId, reservation.value]);

    useEffect(() => {
        if (car) {
            setRemainingReach(car.remainingReach);
        }
    }, [car]);

    useEffect(() => {
        const interval = setInterval(() => {
            setBattery((b) => Math.max(0, b - Math.random() * 0.2));
            setTemperature((t) => t + (Math.random() * 0.5 - 0.25));
        }, 1500);

        return () => clearInterval(interval);
    }, [speed]);

    useEffect(() => {
        const threshold = 0.3;
        const stopped = stopTimes.some(
            (t) => Math.abs(t - currentTime) < threshold
        );
        if (stopped) {
            setSpeed(0);
            setLastMoveTime(null);
        } else {
            if (lastMoveTime === null) {
                setLastMoveTime(currentTime);
                setSpeed(0);
            } else {
                const timeSinceMove = currentTime - lastMoveTime;
                const accel = 10; // km/h per second
                const newSpeed = Math.min(maxSpeed, timeSinceMove * accel);
                setSpeed(Math.round(newSpeed));
                setRemainingReach((r) =>
                    r !== undefined
                        ? Math.max(0, r - (newSpeed / 3600) * 4)
                        : undefined
                );
            }
        }
    }, [currentTime, lastMoveTime]);

    const finishDrivingOnChain = useCallback(async () => {
        if (
            !car ||
            !web3.web3 ||
            !web3.account ||
            !web3.contract ||
            !user.value.id
        ) {
            return null;
        }

        try {
            const receipt = await web3.contract.methods
                .returnCar(car.carId, user.value.id)
                .send({
                    from: web3.account,
                    value: web3.web3.utils.toWei("0.01", "ether"),
                });
            const event = receipt.events?.CarReturned?.returnValues;
            const etherValue = web3.web3?.utils.fromWei(
                event.value.toString(),
                "ether"
            );
            dispatch(
                addLog({
                    logId: uuidv4(),
                    name: "Finish Drive on Blockchain",
                    message: `Finished driving car ${car.carId}`,
                    id: receipt.transactionHash,
                    timestamp: Date.now(),
                    value: etherValue,
                })
            );

            return true;
        } catch (error) {
            console.error("Blockchain transaction failed", error);
            return null;
        }
    }, [car, web3.web3, web3.account, web3.contract, user.value.id, dispatch]);

    const finishDrivingHasFailed = useCallback(() => {
        setFeedbackMsg("Failed to finish driving the car.");
        setFeedbackSeverity("error");
        setFeedbackOpen(true);
    }, []);

    const handleFinishDriving = useCallback(async () => {
        const receipt = await finishDrivingOnChain();
        if (!receipt || !reservationCarObject) {
            return finishDrivingHasFailed();
        }
        const response = await apiExecWithToken(BookingApi, (api) =>
            api.apiBookingFinishDrivingPost(reservationCarObject.id)
        );
        if (hasFailed(response)) {
            console.log("HERE 2");
            finishDrivingHasFailed();
            return;
        } else {
            console.log("HERE");
            navigate("/home");
        }
    }, [
        finishDrivingHasFailed,
        finishDrivingOnChain,
        navigate,
        reservationCarObject,
    ]);

    return (
        <Container>
            <NavLinks isLoggedIn={true} />

            <Box className={classes.glassBox}>
                <video
                    src="/videos/driving.mp4"
                    autoPlay
                    muted
                    loop
                    onTimeUpdate={(e) =>
                        setCurrentTime(e.currentTarget.currentTime)
                    }
                    style={{ width: "100%" }}
                />
            </Box>

            <Grid container spacing={4} className={classes.glassBox}>
                <Grid item xs={6} md={3}>
                    <Typography className={classes.label}>Speed</Typography>
                    <Typography className={classes.value}>
                        {speed.toFixed(0)} km/h
                    </Typography>
                </Grid>
                <Grid item xs={6} md={3}>
                    <Typography className={classes.label}>Battery</Typography>
                    <LinearProgress
                        variant="determinate"
                        value={battery}
                        sx={{ height: 10, borderRadius: 5, my: 1 }}
                    />
                    <Typography className={classes.value}>
                        {battery.toFixed(1)}%
                    </Typography>
                </Grid>
                <Grid item xs={6} md={3}>
                    <Typography className={classes.label}>
                        Remaining Reach
                    </Typography>
                    <Typography className={classes.value}>
                        {remainingReach !== undefined
                            ? `${remainingReach.toFixed(1)} km`
                            : "Loading..."}
                    </Typography>
                </Grid>
                <Grid item xs={6} md={3}>
                    <Typography className={classes.label}>
                        Outside Temp
                    </Typography>
                    <Typography className={classes.value}>
                        {temperature.toFixed(1)} °C
                    </Typography>
                </Grid>
            </Grid>
            <Box className={classes.buttonBox}>
                <DefaultButton
                    onClick={handleFinishDriving}
                    buttonclassname={classes.button}
                >
                    Finish Driving
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

export default DrivingPage;
