import React, { useCallback } from "react";
import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";
import ReservationDialog from "./reservation/ReservationDialog.tsx";
import { type CarTO } from "../../api";

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
    handleConfirm: (vehicle: CarTO) => void;
    openDialog: boolean;
    setOpenDialog: (open: boolean) => void;
}

const Vehicle: React.FC<Props> = (props) => {
    const { vehicle, openDialog, setOpenDialog } = props;
    const { classes } = useStyles();

    const handleOpen = () => setOpenDialog(true);
    const handleClose = () => setOpenDialog(false);

    const handleConfirm = useCallback(async () => {
        props.handleConfirm(vehicle);
    }, [vehicle]);

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
        </>
    );
};

export default Vehicle;
