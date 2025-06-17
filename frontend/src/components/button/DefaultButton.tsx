import React from "react";
import type { ButtonProps } from "@mui/material";
import { Button } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    button: {
        padding: "0.75rem",
        backgroundColor: "#00796b !important",
        color: "white",
        fontWeight: "bold",
        borderRadius: "8px",
        transition: "background 0.3s",
        "&:hover": {
            backgroundColor: "#004d40",
        },
    },
}));

type Props = ButtonProps & {
    buttonClassName?: string;
    children: React.ReactNode;
};

const DefaultButton: React.FC<Props> = (props) => {
    const { classes, classNames } = useStyles();
    return (
        <Button
            className={classNames(classes.button, props.buttonClassName)}
            {...props}
        />
    );
};

export default DefaultButton;
