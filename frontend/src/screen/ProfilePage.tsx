import React, { useState } from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import { Box, Typography } from "@mui/material";
import DefaultButton from "../components/button/DefaultButton.tsx";
import NavLinks from "../components/NavLinks.tsx";

const startprofile = {
    firstname: "Eyal",
    lastname: "Johnson",
    email: "Eyal@gmail.com",
    password: "123456",
    address: "Street 123, Berlin",
    walletId: "0xABCDEF1234567890",
    userId: "user_001", 
}

const ProfilePage: React.FC = () => {
    const [profile, setProfile] = useState(startprofile);

    const handleChange = (field: string, value: string) => {
        setProfile((prev) => ({ ...prev, [field]: value }));
      };
    
      const handleSave = () => {
        console.log("Saving profile:", profile);
        //send profile to backend API
      };

    return (
        <Container center>
            <Typography variant="h4" gutterBottom>Profile Settings</Typography>

            <DefaultTextField 
                label={"First Name"} 
                value={profile.firstname} 
                onChange={(e) => handleChange("firstname", e.target.value)}/>

            <DefaultTextField 
                label={"Last Name"} 
                value={profile.lastname} 
                onChange={(e) => handleChange("lastname", e.target.value)}/>

            <DefaultTextField 
                label={"E-mail"} 
                value={startprofile.email}
                onChange={(e) => handleChange("email", e.target.value)}/>

            <DefaultTextField
                label = {"Password"}
                value = {profile.password}
                onChange={(e) => handleChange("password", e.target.value)}/>

            <DefaultTextField
                label="Address"
                value={profile.address}
                onChange={(e) => handleChange("address", e.target.value)}/>

            <DefaultTextField
                label="Wallet ID"
                value={profile.walletId}
                disabled />
            <DefaultTextField
                label="User ID"
                value={profile.userId}
                disabled />

            <Box mt={3}>
                <DefaultButton variant="contained" color="primary" onClick={handleSave}>
                Save Changes
                </DefaultButton>
            </Box>

            <NavLinks isLoggedIn={true}/>
        </Container>
    )
}

export default ProfilePage;
