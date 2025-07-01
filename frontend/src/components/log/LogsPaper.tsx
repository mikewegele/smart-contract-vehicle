import React, { useEffect, useState } from "react";
import CloseIcon from "@mui/icons-material/Close";
import { deleteLog } from "../../store/reducer/logs.ts";
import { makeStyles } from "tss-react/mui";
import { Box, Fade, IconButton, Paper, Stack, Typography } from "@mui/material";
import { useAppDispatch } from "../../store/Store.ts";
import useApiStates from "../../util/useApiStates.ts";

const useStyles = makeStyles()(() => ({
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
    open: boolean;
    setOpen: (open: boolean) => void;
}

const LogsPaper: React.FC<Props> = (props) => {
    const { open, setOpen } = props;

    const { classes } = useStyles();

    const dispatch = useAppDispatch();

    const { logs } = useApiStates("logs");

    const [currentLogs, setCurrentLogs] = useState(logs.value);

    useEffect(() => {
        setCurrentLogs(logs.value);
    }, [logs.value]);

    return (
        <Fade in={open}>
            <Paper className={classes.logWindow} elevation={6}>
                <Box className={classes.logHeaderBox}>
                    <Typography
                        className={classes.logHeader}
                        variant="subtitle1"
                    >
                        Logs of Smart Contract
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
                        currentLogs.length > 0 &&
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
                                            <strong>Type:</strong> {log.name}
                                        </Typography>
                                    )}
                                    {log.id !== undefined && (
                                        <Typography variant="body2">
                                            <strong>TxHash:</strong> {log.id}
                                        </Typography>
                                    )}
                                    {log.message !== undefined && (
                                        <Typography variant="body2">
                                            <strong>Message:</strong>{" "}
                                            {log.message}
                                        </Typography>
                                    )}
                                    {log.value !== undefined && (
                                        <Typography variant="body2">
                                            <strong>ETH Payed:</strong>{" "}
                                            {log.value}ETH
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
                        ))}
                </Box>
            </Paper>
        </Fade>
    );
};

export default LogsPaper;
