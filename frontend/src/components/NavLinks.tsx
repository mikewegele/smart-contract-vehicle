import React from "react";
import makeStyles from "../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    navLinks: {
        display: "flex",
        justifyContent: "center",
        gap: "2rem",
        marginTop: "2rem",
        fontSize: "1.1rem",
        fontWeight: 500,
        color: "#34495e",
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


const NavLinks: React.FC = () => {
    const { classes } = useStyles();
    return (
        <div className={classes.navLinks}>
            <span>Our Cars</span>
            <span>About Us</span>
            <span>Locations</span>
        </div>
    );
};

export default NavLinks;
