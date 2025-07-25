import React, { useCallback, useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import { Alert, Box, Typography } from "@mui/material";
import NavLinks from "../components/NavLinks.tsx";
import { UserApi, type UserProfileUpdateTO } from "../api";
import { useAppDispatch } from "../store/Store.ts";
import { fetchUser } from "../store/reducer/user.ts";
import useApiStates from "../util/useApiStates.ts";
import { apiExecWithToken, hasFailed } from "../util/ApiUtils.ts";
import DefaultButton from "../components/button/DefaultButton.tsx";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    textField: {
        width: "40%",
        margin: "10px 0 !important",
        "& .MuiInputBase-root": {
            background: "rgba(255, 255, 255, 0.5)",
            borderRadius: 8,
            backdropFilter: "blur(8px)",
            border: "1px solid rgba(255, 255, 255, 0.7)",
        },
        "& .MuiInputBase-input": {
            color: "#34495e",
        },
        "& label": {
            color: "#00796b",
        },
    },
}));

const ProfilePage: React.FC = () => {
    const { classes } = useStyles();
    const dispatch = useAppDispatch();
    const [error, setError] = useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = useState(false);

    const { user } = useApiStates("user");

    const [profile, setProfile] = useState<UserProfileUpdateTO>({
        id: null,
        name: null,
        email: null,
        currentPassword: null,
        newPassword: null,
        confirmNewPassword: null,
    });

    useEffect(() => {
        dispatch(fetchUser());
    }, [dispatch]);

    useEffect(() => {
        if (user.value) {
            setProfile({
                id: user.value.id ?? null,
                name: user.value.name ?? null,
                email: user.value.email ?? null,
                currentPassword: null,
                newPassword: null,
                confirmNewPassword: null,
            });
        }
    }, [user]);

    const handleChange = (field: keyof UserProfileUpdateTO, value: string) => {
        setProfile((prev) => ({ ...prev, [field]: value }));
    };

    const handleSave = useCallback(async () => {
        if (!profile) return;
        if (
            profile.newPassword &&
            profile.newPassword !== profile.confirmNewPassword
        ) {
            setError("New password and confirmation do not match.");
            return;
        }
        setError(null);
        setSaveSuccess(false);
        const response = await apiExecWithToken(UserApi, (api) =>
            api.apiUserUpdateProfilePatch(profile)
        );
        if (!hasFailed(response)) {
            setSaveSuccess(true);
            dispatch(fetchUser());
            setProfile((prev) => ({
                ...prev,
                currentPassword: null,
                newPassword: null,
                confirmNewPassword: null,
            }));
        } else {
            setError(response.error.message);
        }
    }, [profile, dispatch]);

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
                label="Name"
                type="text"
                value={profile.name ?? ""}
                onChange={(e) => handleChange("name", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label="E-mail"
                type="email"
                value={profile.email ?? ""}
                onChange={(e) => handleChange("email", e.target.value)}
            />

            <Typography variant="h6" sx={{ mt: 3, mb: 1 }}>
                Change Password
            </Typography>

            <DefaultTextField
                className={classes.textField}
                label="Current Password"
                type="password"
                value={profile.currentPassword ?? ""}
                onChange={(e) =>
                    handleChange("currentPassword", e.target.value)
                }
            />

            <DefaultTextField
                className={classes.textField}
                label="New Password"
                type="password"
                value={profile.newPassword ?? ""}
                onChange={(e) => handleChange("newPassword", e.target.value)}
            />

            <DefaultTextField
                className={classes.textField}
                label="Confirm New Password"
                type="password"
                value={profile.confirmNewPassword ?? ""}
                onChange={(e) =>
                    handleChange("confirmNewPassword", e.target.value)
                }
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
