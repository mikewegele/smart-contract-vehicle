import React from "react";
import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import { type CarTO } from "../../api";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    card: {
        maxWidth: "300px",
        borderRadius: "16px",
        margin: "16px",
        cursor: "pointer",
        background: "rgba(255, 255, 255, 0.4)",
        boxShadow: "0 8px 24px rgba(0, 0, 0, 0.12)",
        backdropFilter: "blur(12px)",
        border: "1px solid rgba(255, 255, 255, 0.6)",
        transition: "box-shadow 0.3s ease",
        "&:hover": {
            boxShadow: "0 12px 36px rgba(0, 0, 0, 0.18)",
        },
    },
    media: {
        height: 180,
        objectFit: "cover",
        borderTopLeftRadius: "16px",
        borderTopRightRadius: "16px",
    },
    content: {
        paddingBottom: "16px !important",
        color: "#34495e",
    },
}));

interface Props {
    vehicle: CarTO;
    clickOnConfirm: (vehicle: CarTO) => Promise<void>;
    handleOpen: (vehicle: CarTO) => void;
}

const Vehicle: React.FC<Props> = (props) => {
    const { vehicle, handleOpen } = props;
    const { classes } = useStyles();

    return (
        <>
            <Card className={classes.card} onClick={() => handleOpen(vehicle)}>
                <CardMedia
                    component="img"
                    image={vehicle.trimImagePath || ""}
                    alt={`${vehicle.companyName} ${vehicle.modelName || ""}`}
                    className={classes.media}
                />
                <CardContent className={classes.content}>
                    <Typography variant="h6">
                        {vehicle.companyName} {vehicle.modelName}
                    </Typography>
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
        </>
    );
};

export default Vehicle;
