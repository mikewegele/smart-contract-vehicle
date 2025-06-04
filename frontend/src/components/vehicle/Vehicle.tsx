import React from "react";
import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    card: {
        maxWidth: "300px",
        borderRadius: "16px",
        boxShadow: "0 4px 12px rgba(0, 0, 0, 0.1)",
        margin: "16px",
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

    return (
        <Card className={classes.card}>
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
    );
};

export default Vehicle;
