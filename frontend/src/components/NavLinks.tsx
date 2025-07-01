import React from "react";
import { useNavigate } from "react-router-dom";
import DefaultButton from "./button/DefaultButton.tsx";
import LogoutIcon from "@mui/icons-material/Logout";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
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
        backdropFilter: "blur(5px)",
        zIndex: 1000,
        backgroundColor: "rgba(255, 255, 255, 0.3)",
        borderBottom: "1px solid rgba(255,255,255,0.2)",
    },
    button: {
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
