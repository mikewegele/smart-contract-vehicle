import React from "react";
import {
    Avatar,
    Box,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Divider,
    Grid,
    Typography,
} from "@mui/material";
import DefaultButton from "../../button/DefaultButton.tsx";
import type { CarTO } from "../../../api";
import makeStyles from "../../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    paper: {
        background: "rgba(255, 255, 255, 0.75)",
        backdropFilter: "blur(20px)",
        borderRadius: 20,
        border: "1.5px solid rgba(255, 255, 255, 0.9)",
        boxShadow: "0 10px 30px rgba(0, 0, 0, 0.06)",
    },
    dialogTitle: {
        fontWeight: 600,
        fontSize: "1.8rem",
        letterSpacing: "0.03em",
    },
    questionText: {
        fontWeight: 500,
        fontSize: "1.2rem",
        marginTop: "1.5rem",
    },
}));

interface Props {
    open: boolean;
    onClose: () => void;
    onConfirm: () => void;
    car: CarTO;
}

const ReservationDialog: React.FC<Props> = ({
    open,
    onClose,
    onConfirm,
    car,
}) => {
    const { classes } = useStyles();

    return (
        <Dialog
            classes={{ paper: classes.paper }}
            open={open}
            onClose={onClose}
            maxWidth="sm"
            fullWidth
        >
            <DialogTitle className={classes.dialogTitle}>
                Reserve Vehicle
            </DialogTitle>
            <DialogContent>
                <Box display="flex" alignItems="center" gap={2} mb={2}>
                    {car.companyLogoPath && (
                        <Avatar
                            variant="rounded"
                            sx={{
                                width: 56,
                                height: 56,
                                bgcolor: "#34495e",
                                color: "#fff",
                                fontWeight: 600,
                            }}
                        >
                            {car.companyName?.[0]?.toUpperCase() || "?"}
                        </Avatar>
                    )}
                    <Box>
                        <Typography variant="h6">
                            {car.companyName} {car.modelName} {car.trimName}
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            {car.fueltypeName} • {car.drivetrainName} •{" "}
                            {car.seats} seats
                        </Typography>
                    </Box>
                </Box>

                {car.trimImagePath && (
                    <Box
                        component="img"
                        src={car.trimImagePath}
                        alt="Car"
                        sx={{ width: "100%", borderRadius: 2, mb: 2 }}
                    />
                )}

                <Divider sx={{ mb: 2 }} />

                <Grid container spacing={2}>
                    <Grid item xs={6}>
                        <Typography variant="body2" color="textSecondary">
                            Price per minute:
                        </Typography>
                        <Typography variant="body1">
                            €{car.pricePerMinute.toFixed(2)}
                        </Typography>
                    </Grid>
                    {car.remainingReach !== undefined && (
                        <Grid item xs={6}>
                            <Typography variant="body2" color="textSecondary">
                                Remaining range:
                            </Typography>
                            <Typography variant="body1">
                                {car.remainingReach} km
                            </Typography>
                        </Grid>
                    )}
                </Grid>

                <Typography
                    className={classes.questionText}
                    variant="body1"
                    mt={3}
                >
                    Do you want to reserve this vehicle?
                </Typography>
            </DialogContent>

            <DialogActions>
                <DefaultButton onClick={onClose} color="secondary">
                    Cancel
                </DefaultButton>
                <DefaultButton
                    onClick={onConfirm}
                    color="primary"
                    variant="contained"
                >
                    Reserve
                </DefaultButton>
            </DialogActions>
        </Dialog>
    );
};

export default ReservationDialog;
