import React, {useEffect, useState} from "react";
import makeStyles from "../util/makeStyles.ts";
import Header from "./Header";
import NavLinks from "./NavLinks";
import AuthForm from "./AuthForm";
import { getSmartContractVehicle } from '../api';

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

    async function loadVehicle() {
        try {
            const response = await getSmartContractVehicle().get();
            console.log('Daten:', response);
        } catch (error) {
            console.error('Fehler beim Laden:', error);
        }
    }

    useEffect(() => {
        loadVehicle();
    }, [])

    return (
        <div className={classes.container}>
            <Header />
            <AuthForm
                isLogin={isLogin}
                onToggle={handleSubmit}
                error={error}
                setIsLogin={setIsLogin}
            />
            <NavLinks />

        </div>
    );
};

export default EntryPage;
