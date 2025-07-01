import React from "react";
import type { ButtonProps } from "@mui/material";
import { Button } from "@mui/material";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    button: {
        padding: "0.75rem",
        background: "rgba(0, 121, 107, 0.35)",
        color: "white",
        fontWeight: "bold",
        borderRadius: "16px",
        backdropFilter: "blur(12px)",
        border: "1px solid rgba(0, 150, 135, 0.7)",
        boxShadow:
            "inset 0 1px 0 rgba(255,255,255,0.7), 0 6px 10px rgba(0,0,0,0.15)",
        transition: "all 0.3s ease",
        cursor: "pointer",
        userSelect: "none",
        "&:hover": {
            background: "rgba(0, 121, 107, 0.5)",
            boxShadow:
                "inset 0 1px 0 rgba(255,255,255,0.9), 0 10px 14px rgba(0,0,0,0.2)",
            transform: "translateY(-2px)",
        },

        "&:active": {
            background: "rgba(0, 121, 107, 0.25)",
            boxShadow: "inset 0 2px 6px rgba(0,0,0,0.25)",
            transform: "translateY(1px)",
        },
    },
}));

type Props = ButtonProps & {
    buttonclassname?: string;
    children: React.ReactNode;
};

const DefaultButton: React.FC<Props> = (props) => {
    const { classes, cx } = useStyles();
    return (
        <Button
            className={cx(classes.button, props.buttonclassname)}
            {...props}
        />
    );
};

export default DefaultButton;
