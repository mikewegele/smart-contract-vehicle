import React, {useEffect} from "react";
import makeStyles from "../util/makeStyles.ts";
import Header from "./Header";
import NavLinks from "./NavLinks";
import AuthForm from "./AuthForm";
import { getSmartContractVehicle } from '../api';

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        padding: "2rem",
    },
}));

const EntryPage: React.FC = () => {
    const { classes } = useStyles();

    return (
        <div className={classes.container}>
            <Header />
            <AuthForm/>
            <NavLinks />
        </div>
    );
};

export default EntryPage;
