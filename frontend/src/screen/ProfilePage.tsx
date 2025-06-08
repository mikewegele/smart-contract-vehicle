import React, { useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import { Alert, Box, CircularProgress, Typography } from "@mui/material";
import DefaultButton from "../components/button/DefaultButton.tsx";
import NavLinks from "../components/NavLinks.tsx";
import makeStyles from "../util/makeStyles.ts";
import type {UserTO} from "../api";

const useStyles = makeStyles(() => ({
    textField: {
        width: "400px",
        margin: "10px !important",
    },
}));

const backendUrl = "http://localhost:5147"; // Change if needed

const ProfilePage: React.FC = () => {
    const { classes } = useStyles();

    const [profile, setProfile] = useState<UserTO>({
        userId: "",
        username: "",
        email: "",
        name: "",
        password: "",
        address: "",
        walletId: "",
    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = useState(false);

    useEffect(() => {

    }, []);

    const handleChange = (field: keyof UserTO, value: string) => {
        setProfile((prev) => ({ ...prev, [field]: value }));
    };

    if (loading) {
        return (
            <Container>
                <Box display="flex" justifyContent="center" mt={5}>
                    <CircularProgress />
                </Box>
            </Container>
        );
    }

    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                Profile Settings
            </Typography>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            {saveSuccess && (
                <Alert severity="success" sx={{ mb: 2 }}>
                    Profile updated successfully!
                </Alert>
            )}

            <DefaultTextField
                className={classes.textField}
                label={"Username"}
                type={"text"}
                value={profile.userName}
                onChange={(e) => handleChange("username", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label={"E-mail"}
                type="email"
                value={profile.email}
                onChange={(e) => handleChange("email", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label="Name"
                type="text"
                value={profile.name}
                onChange={(e) => handleChange("name", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label="Address"
                value={profile.}
                onChange={(e) => handleChange("address", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label="Wallet ID"
                value={profile.walletId}
                disabled
            />

            <DefaultTextField
                className={classes.textField}
                label="User ID"
                value={profile.userId}
                disabled
            />

            <Box mt={3}>
                <DefaultButton
                    variant="contained"
                    color="primary"
                    onClick={handleSave}
                >
                    Save Changes
                </DefaultButton>
            </Box>

            <NavLinks isLoggedIn={true} />
        </Container>
    );
};

export default ProfilePage;
