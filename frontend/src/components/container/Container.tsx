import React, { type PropsWithChildren, useState } from "react";
import { makeStyles } from "tss-react/mui";
import MenuIcon from "@mui/icons-material/Menu";

import DefaultButton from "../button/DefaultButton.tsx";
import LogsPaper from "../log/LogsPaper.tsx";

const useStyles = makeStyles()(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        minHeight: "100%",
        height: "auto",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        position: "relative",
    },
    center: {
        alignItems: "center",
        justifyContent: "center",
    },
    box: {
        display: "flex",
        flexDirection: "column",
        padding: "1rem",
        flexGrow: 1,
        paddingTop: "6rem",
    },
    button: {
        position: "fixed",
        bottom: "1rem",
        right: "1rem",
        color: "#34495e",
        background: "rgba(255, 255, 255, 0.25)",
        borderRadius: "16px",
        padding: "0.6rem 1.6rem",
        fontWeight: 600,
        backdropFilter: "blur(10px)",
        border: "1px solid rgba(255, 255, 255, 0.3)",
        transition: "all 0.3s ease",
        cursor: "pointer",
        userSelect: "none",
        boxShadow:
            "inset 0 1px 0 rgba(255,255,255,0.6), 0 4px 6px rgba(0,0,0,0.1)",
        "&:hover": {
            background: "rgba(255, 255, 255, 0.4)",
            boxShadow:
                "inset 0 1px 0 rgba(255,255,255,0.8), 0 8px 12px rgba(0,0,0,0.15)",
            transform: "translateY(-2px)",
        },
        "&:active": {
            background: "rgba(255, 255, 255, 0.2)",
            boxShadow: "inset 0 2px 6px rgba(0,0,0,0.2)",
            transform: "translateY(1px)",
        },
    },
}));

interface Props {
    center?: boolean;
    className?: string;
}

const Container: React.FC<PropsWithChildren<Props>> = (props) => {
    const { classes, cx } = useStyles();
    const [open, setOpen] = useState(false);

    return (
        <div
            className={cx(
                classes.container,
                props.center ? classes.center : ""
            )}
        >
            <div className={classes.box}>{props.children}</div>
            <DefaultButton
                className={classes.button}
                onClick={() => setOpen(!open)}
                endIcon={<MenuIcon />}
            >
                Logs of Smart Contract
            </DefaultButton>
            <LogsPaper open={open} setOpen={setOpen} />
        </div>
    );
};

export default Container;
