import React from "react";
import Vehicle from "./Vehicle";
import type { CarTO } from "../../api";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    container: {
        display: "flex",
        flexWrap: "wrap",
    },
}));

interface Props {
    vehicles: CarTO[];
    clickOnConfirm: (vehicle: CarTO) => Promise<void>;
    handleOpen: (vehicle: CarTO) => void;
}

const VehicleList: React.FC<Props> = (props) => {
    const { vehicles, clickOnConfirm, handleOpen } = props;

    const { classes } = useStyles();

    return (
        <div className={classes.container}>
            {vehicles.map((car) => (
                <Vehicle
                    key={car.carId}
                    vehicle={car}
                    handleOpen={handleOpen}
                    clickOnConfirm={clickOnConfirm}
                />
            ))}
        </div>
    );
};

export default VehicleList;
