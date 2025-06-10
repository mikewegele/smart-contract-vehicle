import React, { useCallback, useState } from "react";
import { Card, CardContent, CardMedia, Typography } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";
import ReservationDialog from "./reservation/ReservationDialog.tsx";
import { useWeb3 } from "../../web3/Web3Provider.tsx";

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

    const { account, web3, contract } = useWeb3();

    const [openDialog, setOpenDialog] = useState(false);

    const handleOpen = () => setOpenDialog(true);
    const handleClose = () => setOpenDialog(false);

    const handleConfirm = useCallback(async () => {
        try {
            const carId = 1;
            const numberOfDays = 2;
            const pricePerDay = (pricePerMinute || 0) * 60 * 24;
            const totalCostEther = 0.1; // oder: pricePerDay * numberOfDays in Ether
            const totalCost = web3?.utils.toWei(
                totalCostEther.toString(),
                "ether"
            );
            if (!account || !contract) {
                console.error("No account or contract loaded.");
                return;
            }
            await contract.methods
                .rentCar(carId, numberOfDays)
                .send({ from: account, value: totalCost });

            console.log("Car reserved!");
        } catch (err) {
            console.error("Error reserving car:", err);
        } finally {
            setOpenDialog(false);
        }
    }, [account, contract, pricePerMinute, web3]);

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
