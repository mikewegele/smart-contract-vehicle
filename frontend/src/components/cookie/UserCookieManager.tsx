import React, { useEffect, useState } from "react";
import { useCookies } from "react-cookie";
import {
    Box,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
    Typography,
} from "@mui/material";
import { makeStyles } from "tss-react/mui";
import DefaultButton from "../button/DefaultButton.tsx";

const useStyles = makeStyles()(() => ({
    root: {
        textAlign: "center",
        marginTop: "24px",
        fontFamily: "'Roboto', sans-serif",
    },
    paper: {
        background: "rgba(255, 255, 255, 0.5) !important",
        backdropFilter: "blur(2px) !important",
        WebkitBackdropFilter: "blur(2px) !important",
        boxShadow: "0 2px 8px rgba(0, 0, 0, 0.05) !important",
        borderRadius: "16px !important",
        color: "#233241 !important",
    },
    dialogContent: {
        paddingTop: "8px",
        paddingBottom: "24px",
        color: "#233241",
    },
    dialogTitle: {
        display: "flex",
        alignItems: "center",
        gap: "10px",
        fontWeight: 600,
        color: "#233241",
    },
    btn: {
        margin: "8px",
        borderRadius: "20px",
        paddingLeft: "24px",
        paddingRight: "24px",
        background: "rgba(255, 255, 255, 0.6)",
        color: "#233241",
        backdropFilter: "blur(1px)",
        border: "1px solid rgba(255, 255, 255, 0.6)",
        transition: "background 0.3s",
        "&:hover": {
            background: "rgba(255, 255, 255, 0.7)",
        },
    },
}));

const UserCookieManager: React.FC = () => {
    const { classes } = useStyles();
    const [cookies, setCookie] = useCookies(["user"]);
    const [open, setOpen] = useState(false);

    useEffect(() => {
        if (!cookies.user) {
            setOpen(true);
        } else {
            setOpen(false);
        }
    }, [cookies.user]);

    const handleSetCookie = () => {
        setCookie("user", "Mike", {
            path: "/",
            maxAge: 60 * 60 * 24 * 7,
            sameSite: "lax",
        });
        setOpen(false);
    };

    return (
        <Box className={classes.root}>
            <Dialog
                open={open}
                onClose={() => {}}
                disableEscapeKeyDown
                classes={{ paper: classes.paper }}
            >
                <DialogTitle className={classes.dialogTitle}>
                    Cookie not set
                </DialogTitle>
                <DialogContent className={classes.dialogContent}>
                    <Typography variant="body1">
                        No user cookie was found. Would you like to set one to
                        improve the user experience?
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Stack
                        direction="row"
                        spacing={2}
                        paddingRight="16px"
                        paddingBottom="8px"
                    >
                        <DefaultButton
                            onClick={handleSetCookie}
                            variant="contained"
                            color="primary"
                        >
                            Accept Cookies
                        </DefaultButton>
                    </Stack>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default UserCookieManager;
