import React, { type PropsWithChildren, useEffect, useState } from "react";
import { Box, Fade, IconButton, Paper, Stack, Typography } from "@mui/material";
import { makeStyles } from "tss-react/mui";
import CloseIcon from "@mui/icons-material/Close";
import MenuIcon from "@mui/icons-material/Menu";

import DefaultButton from "../button/DefaultButton.tsx";
import useApiStates from "../../util/useApiStates.ts";
import { useAppDispatch } from "../../store/Store.ts";
import { deleteLog } from "../../store/reducer/logs.ts";

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
    logWindow: {
        position: "fixed",
        top: "1rem",
        bottom: "1rem",
        right: "1rem",
        width: "360px",
        height: "calc(100vh - 2rem)",
        display: "flex",
        flexDirection: "column",
        zIndex: 1299,
        borderRadius: "16px",
        overflow: "hidden",
        background: "rgba(255, 255, 255, 0.08)",
        backdropFilter: "blur(14px)",
        WebkitBackdropFilter: "blur(14px)",
        boxShadow: "0 8px 32px rgba(0, 0, 0, 0.35)",
        border: "1px solid rgba(255, 255, 255, 0.18)",
    },
    logHeaderBox: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "0.75rem 1rem",
        background: "rgba(255, 255, 255, 0.1)",
        backdropFilter: "blur(8px)",
        WebkitBackdropFilter: "blur(8px)",
    },
    logHeader: {
        color: "#34495e",
        fontWeight: 600,
        fontSize: "1.5rem",
    },
    logContent: {
        flex: 1,
        overflowY: "auto",
        padding: "1rem",
        backgroundColor: "rgba(255, 255, 255, 0.2)",
        color: "#34495e",
        fontWeight: 600,
        lineHeight: 1.5,
    },
    logItem: {
        marginBottom: "0.5rem",
        fontSize: "0.8rem",
        maxWidth: "600px",
        padding: "0.75rem",
        borderRadius: "8px",
        background: "rgba(255, 255, 255, 0.1)",
        backdropFilter: "blur(6px)",
        WebkitBackdropFilter: "blur(6px)",
        border: "1px solid rgba(255, 255, 255, 0.2)",
        overflowWrap: "break-word",
        boxShadow: "0 2px 8px rgba(0, 0, 0, 0.2)",
    },
}));

interface Props {
    center?: boolean;
    className?: string;
}

const Container: React.FC<PropsWithChildren<Props>> = (props) => {
    const { classes, cx } = useStyles();
    const [open, setOpen] = useState(false);

    const dispatch = useAppDispatch();

    const { logs } = useApiStates("logs");

    const [currentLogs, setCurrentLogs] = useState(logs.value);

    useEffect(() => {
        setCurrentLogs(logs.value);
    }, [logs.value]);

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
                Logs
            </DefaultButton>
            <Fade in={open}>
                <Paper className={classes.logWindow} elevation={6}>
                    <Box className={classes.logHeaderBox}>
                        <Typography
                            className={classes.logHeader}
                            variant="subtitle1"
                        >
                            Logs
                        </Typography>
                        <IconButton
                            size="small"
                            onClick={() => setOpen(false)}
                            sx={{ color: "#34495e" }}
                        >
                            <CloseIcon />
                        </IconButton>
                    </Box>
                    <Box className={classes.logContent}>
                        {Array.isArray(currentLogs) &&
                        currentLogs.length > 0 ? (
                            currentLogs.map((log, index) => (
                                <Paper
                                    key={index}
                                    className={classes.logItem}
                                    elevation={3}
                                >
                                    <IconButton
                                        size="small"
                                        onClick={() => {
                                            const updated = [...currentLogs];
                                            updated.splice(index, 1);
                                            setCurrentLogs(updated);
                                            if (log.logId) {
                                                dispatch(deleteLog(log.logId));
                                            }
                                        }}
                                        sx={{
                                            position: "absolute",
                                            top: 4,
                                            right: 4,
                                            color: "#888",
                                        }}
                                    >
                                        <CloseIcon fontSize="small" />
                                    </IconButton>
                                    <Stack spacing={0.5}>
                                        {log.name !== undefined && (
                                            <Typography variant="body2">
                                                <strong>Type:</strong>{" "}
                                                {log.name}
                                            </Typography>
                                        )}
                                        {log.id !== undefined && (
                                            <Typography variant="body2">
                                                <strong>TxHash:</strong>{" "}
                                                {log.id}
                                            </Typography>
                                        )}
                                        {log.message !== undefined && (
                                            <Typography variant="body2">
                                                <strong>Message:</strong>{" "}
                                                {log.message}
                                            </Typography>
                                        )}
                                        {log.timestamp !== undefined && (
                                            <Typography variant="body2">
                                                <strong>Timestamp:</strong>{" "}
                                                {log.timestamp}
                                            </Typography>
                                        )}
                                    </Stack>
                                </Paper>
                            ))
                        ) : (
                            <Typography variant="body2" color="textSecondary">
                                No Logs
                            </Typography>
                        )}
                    </Box>
                </Paper>
            </Fade>
        </div>
    );
};

export default Container;
