import React from "react";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    mainTitle: {
        textAlign: "center",
        fontSize: "3rem",
        fontWeight: "bold",
        marginBottom: "2rem",
        color: "#233241",
    },
}));

const Header: React.FC = () => {
    const { classes } = useStyles();
    return <h1 className={classes.mainTitle}>Smart Car Rental</h1>;
};

export default Header;
