import React from "react";
import Vehicle from "./Vehicle";
import makeStyles from "../../util/makeStyles.ts";
import type { CarTO } from "../../api";

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

    const { classes } = useStyles();

    return (
        <div className={classes.container}>
            {vehicles.map((car) => (
                <Vehicle
                    key={car.carId}
                    image={car.trimImagePath}
                    seats={car.seats}
                    model={car.modelName}
                    pricePerMinute={car.pricePerMinute}
                    rangeKm={car.remainingReach}
                />
            ))}
        </div>
    );
};

export default VehicleList;
