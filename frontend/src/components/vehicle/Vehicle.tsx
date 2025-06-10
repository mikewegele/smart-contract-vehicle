import React, { useCallback, useState } from "react";
import {
    Alert,
    Card,
    CardContent,
    CardMedia,
    Snackbar,
    Typography,
} from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";
import ReservationDialog from "./reservation/ReservationDialog.tsx";
import { useWeb3 } from "../../web3/Web3Provider.tsx";
import type { CarTO } from "../../api";

const useStyles = makeStyles(() => ({
    card: {
        maxWidth: "300px",
        borderRadius: "16px",
        boxShadow: "0 4px 12px rgba(0, 0, 0, 0.1)",
        margin: "16px",
        cursor: "pointer",
    },
    media: {
        height: 180,
        objectFit: "cover",
    },
    content: {
        paddingBottom: "16px !important",
    },
}));

interface Props {
    vehicle: CarTO;
}

const Vehicle: React.FC<Props> = (props) => {
    const { vehicle } = props;
    const { classes } = useStyles();

    const { account, web3, contract } = useWeb3();

    const [openDialog, setOpenDialog] = useState(false);
    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");

    const handleOpen = () => setOpenDialog(true);
    const handleClose = () => setOpenDialog(false);

    const handleConfirm = useCallback(async () => {
        try {
            const carId = 1;
            const numberOfDays = 2;
            // const pricePerDay = (vehicle.pricePerMinute || 0) * 60 * 24;
            const totalCostEther = 0.1; // oder: pricePerDay * numberOfDays in Ether
            const totalCost = web3?.utils.toWei(
                totalCostEther.toString(),
                "ether"
            );

            if (!account || !contract) {
                setFeedbackMsg("No account or contract loaded.");
                setFeedbackSeverity("error");
                setFeedbackOpen(true);
                return;
            }

            await contract.methods
                .rentCar(carId, numberOfDays)
                .send({ from: account, value: totalCost });

            setFeedbackMsg("Car successfully reserved!");
            setFeedbackSeverity("success");
        } catch (err) {
            console.error("Error reserving car:", err);
            setFeedbackMsg("Failed to reserve car.");
            setFeedbackSeverity("error");
        } finally {
            setFeedbackOpen(true);
            setOpenDialog(false);
        }
    }, [account, contract, vehicle.pricePerMinute, web3]);

    return (
        <>
            <Card className={classes.card} onClick={handleOpen}>
                <CardMedia
                    component="img"
                    image={vehicle.trimImagePath || ""}
                    alt={vehicle.modelName || ""}
                    className={classes.media}
                />
                <CardContent className={classes.content}>
                    <Typography variant="h6">{vehicle.modelName}</Typography>
                    <Typography variant="body2" color="textSecondary">
                        {vehicle.pricePerMinute} € / Minute
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        Seats: {vehicle.seats}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        Distance: {vehicle.remainingReach} km
                    </Typography>
                </CardContent>
            </Card>
            <ReservationDialog
                open={openDialog}
                onClose={handleClose}
                onConfirm={handleConfirm}
                car={vehicle}
            />
            <Snackbar
                open={feedbackOpen}
                autoHideDuration={4000}
                onClose={() => setFeedbackOpen(false)}
                anchorOrigin={{ vertical: "top", horizontal: "center" }}
            >
                <Alert
                    severity={feedbackSeverity}
                    onClose={() => setFeedbackOpen(false)}
                    variant="filled"
                >
                    {feedbackMsg}
                </Alert>
            </Snackbar>
        </>
    );
};

export default Vehicle;
