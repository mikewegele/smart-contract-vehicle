import React from "react";
import { Button } from "@mui/material";
import type { ButtonProps } from "@mui/material"
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    button: {
        padding: "0.75rem",
        backgroundColor: "#00796b !important",
        color: "white",
        fontWeight: "bold",
        borderRadius: 8,
        transition: "background 0.3s",
        "&:hover": {
            backgroundColor: "#004d40",
        },
    },
}));

type Props = ButtonProps & {
    children: React.ReactNode;
};

const DefaultButton: React.FC<Props> = (props) => {
    const { classes } = useStyles();
    return <Button className={classes.button} {...props} />;
};

export default DefaultButton;
