import React from "react";
import makeStyles from "../util/makeStyles.ts";
import { useNavigate } from "react-router-dom";
import DefaultButton from "./button/DefaultButton.tsx";
import LogoutIcon from "@mui/icons-material/Logout";

const useStyles = makeStyles(() => ({
    navLinks: {
        position: "fixed",
        top: 0,
        left: 0,
        width: "100%",
        display: "flex",
        justifyContent: "center",
        gap: "2rem",
        padding: "1rem 0",
        fontSize: "1.1rem",
        fontWeight: 500,
        color: "#34495e",
        boxShadow: "0 2px 5px rgba(0,0,0,0.1)",
    },
    button: {
        color: "white",
        backgroundColor: "#34495e",
    },
}));

interface Props {
    isLoggedIn: boolean;
}

const NavLinks: React.FC<Props> = ({ isLoggedIn }) => {
    const { classes } = useStyles();
    const navigate = useNavigate();
    return (
        <div className={classes.navLinks}>
            <DefaultButton
                className={classes.button}
                onClick={() => navigate("/home")}
            >
                Home
            </DefaultButton>
            <DefaultButton className={classes.button}>About Us</DefaultButton>
            <DefaultButton
                className={classes.button}
                onClick={() => navigate("/smart")}
            >
                Smart
            </DefaultButton>
            <DefaultButton className={classes.button}>Locations</DefaultButton>
            {isLoggedIn && (
                <DefaultButton
                    className={classes.button}
                    onClick={() => navigate("/profile")}
                >
                    Profile
                </DefaultButton>
            )}
            {isLoggedIn && (
                <DefaultButton
                    endIcon={<LogoutIcon />}
                    onClick={() => {
                        localStorage.removeItem("token");
                        navigate("/login");
                    }}
                >
                    Logout
                </DefaultButton>
            )}
        </div>
    );
};

export default NavLinks;
