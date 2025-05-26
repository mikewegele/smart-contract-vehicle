import React, {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {Box, Dialog, DialogActions, DialogContent, DialogTitle, Stack, Typography,} from "@mui/material";
import makeStyles from "../../util/makeStyles";
import DefaultButton from "../button/DefaultButton.tsx";

const useStyles = makeStyles(() => ({
    root: {
        textAlign: "center",
        marginTop: "24px",
        fontFamily: "'Roboto', sans-serif",
    },
    btn: {
        margin: "8px",
        borderRadius: "20px",
        paddingLeft: "24px",
        paddingRight: "24px",
    },
    dialogContent: {
        paddingTop: "8px",
        paddingBottom: "24px",
    },
    dialogTitle: {
        display: "flex",
        alignItems: "center",
        gap: "10px",
        fontWeight: 600,
    },
    icon: {
        fontSize: "28px",
        color: "#f9a825",
    },
}));

const UserCookieManager: React.FC = () => {

    const {classes} = useStyles();
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
                onClose={() => {
                }}
                disableEscapeKeyDown
            >
                <DialogTitle className={classes.dialogTitle}>
                    Cookie not set
                </DialogTitle>
                <DialogContent className={classes.dialogContent}>
                    <Typography variant="body1">
                        No user cookie was found. Would you like to set one to improve the user experience?
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Stack direction="row" spacing={2} paddingRight="16px" paddingBottom="8px">
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
