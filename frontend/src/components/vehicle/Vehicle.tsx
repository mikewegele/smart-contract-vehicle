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
import { CarApi, type CarTO } from "../../api";
import type { IWeb3Context } from "../../web3/Web3Provider.tsx";
import useApiStates from "../../util/useApiStates.ts";
import { apiExec, hasFailed } from "../../util/ApiUtils.ts";
import { useAppDispatch } from "../../store/Store.ts";
import { setReservedCar } from "../../store/reducer/cars.ts";
import { useNavigate } from "react-router-dom";

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
    web3Context: IWeb3Context;
}

const Vehicle: React.FC<Props> = (props) => {
    const { vehicle, web3Context } = props;
    const { classes } = useStyles();

    const [openDialog, setOpenDialog] = useState(false);
    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");

    const { user } = useApiStates("user");

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const handleOpen = () => setOpenDialog(true);
    const handleClose = () => setOpenDialog(false);

    const reservationHasFailed = useCallback(() => {
        setFeedbackMsg("Failed to reserve car.");
        setFeedbackSeverity("error");
    }, []);

    const handleConfirm = useCallback(async () => {
        const response = await apiExec(CarApi, (api) =>
            api.apiCarReserveCarGet(vehicle.carId)
        );

        if (hasFailed(response)) {
            reservationHasFailed();
            return;
        }

        if (
            !web3Context.web3 ||
            !web3Context.account ||
            !web3Context.contract ||
            !user.value.id ||
            !vehicle.pricePerMinute
        ) {
            setFeedbackMsg("No account or contract loaded.");
            setFeedbackSeverity("error");
            setFeedbackOpen(true);
            return;
        }

        const receipt = await web3Context.contract.methods
            .rentCar(vehicle.carId, user.value.id)
            .send({
                from: web3Context.account,
                value: web3Context.web3.utils.toWei("0.01", "ether"),
            })
            .catch(() => {
                reservationHasFailed();
                return;
            });

        if (receipt) {
            const event = receipt.events.CarRented.returnValues;
            const transactionHash = receipt.transactionHash;
            console.log(transactionHash);
            dispatch(setReservedCar(vehicle));
            navigate(`/reservation/${vehicle.carId}`);
            setFeedbackMsg("Car successfully reserved!");
            setFeedbackSeverity("success");
        }

        setFeedbackOpen(true);
        setOpenDialog(false);
    }, [
        web3Context.web3,
        web3Context.account,
        web3Context.contract,
        user.value.id,
        vehicle,
        reservationHasFailed,
    ]);

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
