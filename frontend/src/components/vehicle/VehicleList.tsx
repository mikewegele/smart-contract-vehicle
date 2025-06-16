import React, { useCallback, useState } from "react";
import Vehicle from "./Vehicle";
import makeStyles from "../../util/makeStyles.ts";
import { CarApi, type CarTO } from "../../api";
import { useWeb3 } from "../../web3/Web3Provider.tsx";
import { apiExec, hasFailed } from "../../util/ApiUtils.ts";
import useApiStates from "../../util/useApiStates.ts";
import { Alert, Snackbar } from "@mui/material";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexWrap: "wrap",
    },
}));

interface Props {
    vehicles: CarTO[];
}

const VehicleList: React.FC<Props> = (props) => {
    const { vehicles } = props;

    const web3Context = useWeb3();

    const { classes } = useStyles();

    const { user } = useApiStates("user");

    const [openDialog, setOpenDialog] = useState(false);
    const [feedbackOpen, setFeedbackOpen] = useState(false);
    const [feedbackMsg, setFeedbackMsg] = useState("");
    const [feedbackSeverity, setFeedbackSeverity] = useState<
        "success" | "error"
    >("success");

    const handleConfirm = useCallback(
        async (vehicle: CarTO) => {
            const result = await apiExec(CarApi, (api) =>
                api.apiCarReserveCarGet(vehicle.carId)
            );
            if (!hasFailed(result)) {
                try {
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
                            value: web3Context.web3?.utils.toWei(
                                "0.01",
                                "ether"
                            ),
                        });

                    const event = receipt.events.CarRented.returnValues;

                    console.log(event);

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
            }
        },
        [
            user.value.id,
            web3Context.account,
            web3Context.contract,
            web3Context.web3,
        ]
    );

    return (
        <>
            <div className={classes.container}>
                {vehicles.map((car) => (
                    <Vehicle
                        key={car.carId}
                        vehicle={car}
                        handleConfirm={handleConfirm}
                        openDialog={openDialog}
                        setOpenDialog={setOpenDialog}
                    />
                ))}
            </div>
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

export default VehicleList;
