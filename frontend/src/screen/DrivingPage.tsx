import React, { useEffect, useState } from "react";
import { Box, Grid, LinearProgress, Typography } from "@mui/material";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import { makeStyles } from "tss-react/mui";

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
}));

const stopTimes = [0.3, 5, 10.5, 14, 19.5, 24, 26, 29, 30.5, 35, 38];

const DrivingPage: React.FC = () => {
    const { classes } = useStyles();

    const [speed, setSpeed] = useState(0);
    const [battery, setBattery] = useState(78);
    const [odometer, setOdometer] = useState(21532);
    const [temperature, setTemperature] = useState(27);
    const [currentTime, setCurrentTime] = useState(0);
    const [lastMoveTime, setLastMoveTime] = useState<number | null>(null);
    const maxSpeed = 30;

    useEffect(() => {
        const interval = setInterval(() => {
            setBattery((b) => Math.max(0, b - Math.random() * 0.2));
            setOdometer((o) => o + Math.random() * 0.05);
            setTemperature((t) => t + (Math.random() * 0.5 - 0.25));
        }, 1500);

        return () => clearInterval(interval);
    }, []);

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
            }
        }
    }, [currentTime]);

    return (
        <Container>
            <NavLinks isLoggedIn={true} />

            <Box className={classes.glassBox}>
                <video
                    src="videos/driving.mp4"
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
                    <Typography className={classes.label}>Odometer</Typography>
                    <Typography className={classes.value}>
                        {odometer.toFixed(1)} km
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
        </Container>
    );
};

export default DrivingPage;
