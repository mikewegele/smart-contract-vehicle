import React, { useState } from "react";
import styles from "./EntryPage.module.css";
import makeStyles from "../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        padding: "2rem",
    },
    card: {
        background: "#fff",
        padding: "2rem 2.5rem",
        borderRadius: 12,
        boxShadow: "0 8px 20px rgba(0, 0, 0, 0.1)",
        width: "100%",
        maxWidth: "400px",
    },
    mainTitle: {
        textAlign: "center",
        fontSize: "3rem",
        fontWeight: "bold",
        marginBottom: "2rem",
        color: "#233241",
    },
    title: {
        textAlign: "center",
        marginBottom: "1.5rem",
        color: "#00796b",
        fontSize: "1.8rem",
    },
    form: {
        display: "flex",
        flexDirection: "column",
    },
    input: {
        padding: "0.75rem",
        marginTop: "0.25rem",
        marginBottom: "1rem",
        border: "1px solid #ccc",
        borderRadius: 8,
        transition: "border-color 0.2s",
        "&:focus": {
            outline: "none",
            borderColor: "#00796b",
            boxShadow: "0 0 0 2px rgba(0, 121, 107, 0.2)",
        },
    },
    button: {
        padding: "0.75rem",
        backgroundColor: "#00796b",
        color: "white",
        fontWeight: "bold",
        border: "none",
        borderRadius: 8,
        cursor: "pointer",
        transition: "background 0.3s",
        "&:hover": {
            backgroundColor: "#004d40",
        },
    },
    error: {
        color: "red",
        marginBottom: "1rem",
        textAlign: "center",
    },
    toggle: {
        marginTop: "1rem",
        textAlign: "center",
    },
    link: {
        background: "none",
        border: "none",
        color: "#00796b",
        cursor: "pointer",
        fontWeight: "bold",
        textDecoration: "underline",
        marginLeft: "4px",
        fontSize: "1rem",
        "&:hover": {
            color: "#004d40",
        },
    },
    navLinks: {
        display: "flex",
        justifyContent: "center",
        gap: "2rem",
        marginBottom: "2rem",
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

const EntryPage: React.FC = () => {
    const { classes } = useStyles();

    const [isLogin, setIsLogin] = useState(true);
    const [email, setEmail] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (!email.includes("@")) {
            setError("Email must contain @");
            return;
        }
        if (!isLogin && username.length < 3) {
            setError("Username must be at least 3 characters");
            return;
        }
        if (password.length < 6) {
            setError("Password must be at least 6 characters");
            return;
        }

        setError("");
        if (isLogin) {
            console.log("Logging in with:", { email, password });
        } else {
            console.log("Signing up with:", { email, username, password });
        }
    };

    return (
        <div className={classes.container}>
            <h1 className={classes.mainTitle}>Smart Car Rental</h1>

            <div className={classes.navLinks}>
                <span>Our Cars</span>
                <span>About Us</span>
                <span>Locations</span>
            </div>

            <div className={classes.card}>
                <h2 className={classes.title}>
                    {isLogin ? "Log In" : "Sign Up"}
                </h2>

                <form onSubmit={handleSubmit} className={classes.form}>
                    <label>Email</label>
                    <input
                        className={classes.input}
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />

                    {!isLogin && (
                        <>
                            <label>Username</label>
                            <input
                                className={classes.input}
                                type="text"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </>
                    )}

                    <label>Password</label>
                    <input
                        className={classes.input}
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />

                    {error && <p className={styles.error}>{error}</p>}

                    <button type="submit" className={styles.button}>
                        {isLogin ? "Log In" : "Sign Up"}
                    </button>
                </form>

                <p className={styles.toggle}>
                    {isLogin
                        ? "Don't have an account?"
                        : "Already have an account?"}{" "}
                    <button
                        className={styles.link}
                        onClick={() => setIsLogin(!isLogin)}
                    >
                        {isLogin ? "Sign Up" : "Log In"}
                    </button>
                </p>
            </div>
        </div>
    );
};

export default EntryPage;
