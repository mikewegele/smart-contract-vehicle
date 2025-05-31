import React from "react";
import Vehicle from "./Vehicle";
import makeStyles from "../../util/makeStyles.ts";
import type {VehicleProp} from "../../screen/DashboardPage.tsx";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexWrap: "wrap"
    }
}));

interface Props {
    vehicles: VehicleProp[];
}

const VehicleList: React.FC<Props> = (props) => {

    const {vehicles} = props;

    const {classes} = useStyles();

    return (
        <div className={classes.container}>
            {vehicles.map((v, i) => (
                <Vehicle key={i} {...v} />
            ))}
        </div>
    );
};

export default VehicleList;
