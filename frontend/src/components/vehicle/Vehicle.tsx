import React, { useState } from "react";
import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";
import ReservationDialog from "./reservation/ReservationDialog.tsx";

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
    image: string | null;
    model: string | null;
    pricePerMinute: number | null;
    seats: number;
    rangeKm: number | undefined;
}

const Vehicle: React.FC<Props> = (props) => {
    const { image, model, pricePerMinute, seats, rangeKm } = props;
    const { classes } = useStyles();

    const [openDialog, setOpenDialog] = useState(false);

    const handleOpen = () => setOpenDialog(true);
    const handleClose = () => setOpenDialog(false);
    const handleConfirm = () => {
        setOpenDialog(false);
        console.log(`Vehicle "${model}" has been reserved`);
    };

    return (
        <>
            <Card className={classes.card} onClick={handleOpen}>
                <CardMedia
                    component="img"
                    image={image || ""}
                    alt={model || ""}
                    className={classes.media}
                />
                <CardContent className={classes.content}>
                    <Typography variant="h6">{model}</Typography>
                    <Typography variant="body2" color="textSecondary">
                        {pricePerMinute} € / Minute
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        Seats: {seats}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        Distance: {rangeKm} km
                    </Typography>
                </CardContent>
            </Card>
            <ReservationDialog
                open={openDialog}
                onClose={handleClose}
                onConfirm={handleConfirm}
                model={model}
            />
        </>
    );
};

export default Vehicle;
