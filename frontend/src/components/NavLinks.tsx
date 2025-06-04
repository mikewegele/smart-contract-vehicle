import React from "react";
import makeStyles from "../util/makeStyles.ts";
import { useNavigate } from "react-router-dom";

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
        "& span": {
            cursor: "pointer",
            position: "relative",
            padding: "8px 16px",
            borderRadius: "4px",
            transition: "background-color 0.3s ease, color 0.3s ease",
            "&:hover": {
                color: "#4c838b",
                backgroundColor: "white",
            },
            "&::after": {
                content: '""',
                position: "absolute",
                left: 0,
                bottom: 0,
                height: "2px",
                width: 0,
                backgroundColor: "#4c838b",
                transition: "width 0.3s ease",
            },
            "&:hover::after": {
                width: "100%",
            },
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
            <span onClick={() => navigate("/home")}>Home</span>
            <span>About Us</span>
            <span onClick={() => navigate("/smart")}>Smart</span>
            <span>Locations</span>
            {isLoggedIn && (
                <span onClick={() => navigate("/profile")}>Profile</span>
            )}
        </div>
    );
};

export default NavLinks;
