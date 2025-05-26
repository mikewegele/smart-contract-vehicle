import React, {useState} from "react";
import makeStyles from "../util/makeStyles.ts";
import {apiExec, hasFailed} from "../util/ApiUtils.ts";
import {useNavigate} from "react-router-dom";
import {Alert, Box, Button, Stack, Typography} from "@mui/material";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import DefaultButton from "../components/button/DefaultButton.tsx";

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
        background: "white",
        padding: "2rem 2.5rem",
        borderRadius: "8px",
        border: "1px solid rgba(0, 0, 0, 0.23)",
        width: "400px"
    },
    title: {
        textAlign: "center",
        paddingBottom: "1rem",
        color: "#233241",
        fontSize: "1.8rem",
    },
    form: {
        display: "flex",
        flexDirection: "column",
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

const AuthForm: React.FC = () => {
    const {classes} = useStyles();

    const [isLogin, setIsLogin] = useState(true);
    const [email, setEmail] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
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
            const response = await apiExec(api => api.postApiAuthLoginLogin({Email: email, Password: password}))
            if (!hasFailed(response.status)) {
                navigate("/home");
            } else {
                setError("Failed to log in");
            }
        } else {
            const response = await apiExec(api => api.postApiAuthRegisterRegister({
                Email: email,
                Password: password,
                Name: username
            }));
            if (!hasFailed(response.status)) {
                navigate("/home");
            } else {
                setError("Failed to register");
            }
        }
    };

    return (
        <Box className={classes.card}>
            <Typography className={classes.title} variant="h5">
                {isLogin ? "Log In" : "Sign Up"}
            </Typography>

            <form onSubmit={handleSubmit} className={classes.form}>
                <Stack spacing={2}>
                    <DefaultTextField
                        label="Email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        fullWidth
                        required/>

                    {!isLogin && (
                        <DefaultTextField
                            label="Username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            fullWidth
                            required/>
                    )}

                    <DefaultTextField
                        label="Password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        fullWidth
                        required
                    />
                    {error && <Alert severity="error">{error}</Alert>}

                    <DefaultButton
                        variant="contained"
                        color="primary"
                        type="submit">
                        {isLogin ? "Log In" : "Sign Up"}
                    </DefaultButton>
                </Stack>
            </form>

            <Typography className={classes.toggle}>
                {isLogin ? "Don't have an account?" : "Already have an account?"}
                <Button
                    className={classes.link}
                    onClick={() => setIsLogin(prev => !prev)}
                >
                    {isLogin ? "Sign Up" : "Log In"}
                </Button>
            </Typography>
        </Box>
    );
};

export default AuthForm;
