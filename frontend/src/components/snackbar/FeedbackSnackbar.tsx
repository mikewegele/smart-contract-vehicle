import React from "react";
import { Alert, Snackbar } from "@mui/material";
import { makeStyles } from "tss-react/mui";

interface Props {
    open: boolean;
    message: string;
    severity: "success" | "error";
    onClose: () => void;
}

const useStyles = makeStyles()(() => ({
    root: {
        top: 16,
        bottom: "auto",
    },
}));

const FeedbackSnackbar: React.FC<Props> = (props) => {
    const { open, message, severity, onClose } = props;

    const { classes } = useStyles();

    return (
        <Snackbar
            open={open}
            autoHideDuration={3000}
            onClose={onClose}
            anchorOrigin={{ vertical: "top", horizontal: "center" }}
            className={classes.root}
        >
            <Alert
                onClose={onClose}
                severity={severity}
                variant="filled"
                sx={{ width: "100%" }}
            >
                {message}
            </Alert>
        </Snackbar>
    );
};

export default FeedbackSnackbar;
