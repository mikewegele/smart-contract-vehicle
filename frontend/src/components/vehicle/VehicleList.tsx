import React from "react";
import Vehicle from "./Vehicle";
import type { CarTO } from "../../api";
import { useWeb3 } from "../../web3/Web3Provider.tsx";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
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

    return (
        <div className={classes.container}>
            {vehicles.map((car) => (
                <Vehicle
                    key={car.carId}
                    vehicle={car}
                    web3Context={web3Context}
                />
            ))}
        </div>
    );
};

export default VehicleList;
