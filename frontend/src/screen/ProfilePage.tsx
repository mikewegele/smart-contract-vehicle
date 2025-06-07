import React, { useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import { Box, Typography, CircularProgress, Alert } from "@mui/material";
import DefaultButton from "../components/button/DefaultButton.tsx";
import NavLinks from "../components/NavLinks.tsx";
import makeStyles from "../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
  textField: {
    width: "400px",
    margin: "10px !important",
  },
}));

interface Profile {
  userId: string;
  username: string;
  email: string;
  name: string;
  password: string;
  address: string;
  walletId: string;
}

const backendUrl = "http://localhost:5147"; // Change if needed

const ProfilePage: React.FC = () => {
  const { classes } = useStyles();

  const [profile, setProfile] = useState<Profile>({
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
    const loadUserFromStorage = () => {
      const storedUser = localStorage.getItem("user");
      if (!storedUser) {
        setError("No logged in user found");
        return;
      }

      try {
        const user = JSON.parse(storedUser);
        setProfile((prev) => ({
          ...prev,
          email: user.email || "",
          username: user.userName || "",
          userId: user.id || "",
          name: user.name || "",
        }));

        fetchProfile(user.email);
      } catch {
        setError("Failed to parse stored user data");
      }
    };

    const fetchProfile = async (email: string) => {
      setLoading(true);
      setError(null);

      try {
        const res = await fetch(
          `${backendUrl}/api/User/by-email?email=${encodeURIComponent(email)}`
        );

        if (!res.ok) {
          setError(`Failed to fetch profile (status: ${res.status})`);
          setLoading(false);
          return;
        }

        const userData = await res.json();

        setProfile((prev) => ({
          ...prev,
          userId: userData.id || prev.userId,
          username: userData.userName || prev.username,
          email: userData.email || prev.email,
          name: userData.name || prev.name,
          address: userData.address || "", // if your backend returns this
          walletId: userData.walletId || "",
          password: "", // never populate password
        }));
      } catch (err) {
        setError("Failed to fetch user data");
      } finally {
        setLoading(false);
      }
    };

    loadUserFromStorage();
  }, []);

  const handleChange = (field: keyof Profile, value: string) => {
    setProfile((prev) => ({ ...prev, [field]: value }));
  };

  const handleSave = async () => {
    setError(null);
    setSaveSuccess(false);

    // Basic validation
    if (!profile.email.trim()) {
      setError("Email cannot be empty");
      return;
    }
    if (!profile.username.trim()) {
      setError("Username cannot be empty");
      return;
    }

    const payload: any = {
      id: profile.userId,
      userName: profile.username,
      email: profile.email,
      name: profile.name,
      address: profile.address,
      walletId: profile.walletId,
      isAdmin: false,
      isLessor: false,
      isRenter: false,
    };
    if (profile.password.trim()) {
      payload.password = profile.password;
    }

    try {
      const res = await fetch(`${backendUrl}/api/User/update`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error(`Update failed (status: ${res.status})`);

      const updatedUser = await res.json();

      // Update localStorage user copy if backend returns updated user info
      localStorage.setItem("user", JSON.stringify(updatedUser));

      setSaveSuccess(true);
      setProfile((prev) => ({ ...prev, password: "" })); // clear password
    } catch (err) {
      setError((err as Error).message || "Update failed");
    }
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
        value={profile.username}
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
        label={"Password"}
        type="password"
        value={profile.password}
        onChange={(e) => handleChange("password", e.target.value)}
        helperText="Leave blank to keep current password"
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
        value={profile.address}
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
        <DefaultButton variant="contained" color="primary" onClick={handleSave}>
          Save Changes
        </DefaultButton>
      </Box>

      <NavLinks isLoggedIn={true} />
    </Container>
  );
};

export default ProfilePage;