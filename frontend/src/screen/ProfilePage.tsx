import React, {useState} from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";
import {Box, Typography} from "@mui/material";
import DefaultButton from "../components/button/DefaultButton.tsx";
import NavLinks from "../components/NavLinks.tsx";
import makeStyles from "../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    textField: {
        width: "400px",
        margin: "10px !important",
    },
}));

const START_PROFILE = {
    username: "Eyal",
    email: "Eyal@gmail.com",
    password: "123456",
    address: "Street 123, Berlin",
    walletId: "0xABCDEF1234567890",
    userId: "user_001",
}

const ProfilePage: React.FC = () => {

    const {classes} = useStyles();

    const [profile, setProfile] = useState(START_PROFILE);

    const handleChange = (field: string, value: string) => {
        setProfile((prev) => ({...prev, [field]: value}));
    };

    const handleSave = () => {
        console.log("Saving profile:", profile);
        //send profile to backend API
    };

    return (
        <Container>
            <Typography variant="h4" gutterBottom>Profile Settings</Typography>

            <DefaultTextField
                className={classes.textField}
                label={"Username"}
                type={"name"}
                value={profile.username}
                onChange={(e) => handleChange("username", e.target.value)}/>

            <DefaultTextField
                className={classes.textField}
                label={"E-mail"}
                type="email"
                value={START_PROFILE.email}
                onChange={(e) => handleChange("email", e.target.value)}/>

            <DefaultTextField
                className={classes.textField}
                label={"Password"}
                type="password"
                value={profile.password}
                onChange={(e) => handleChange("password", e.target.value)}/>

            <DefaultTextField
                className={classes.textField}
                label="Address"
                value={profile.address}
                onChange={(e) => handleChange("address", e.target.value)}/>

            <DefaultTextField
                className={classes.textField}
                label="Wallet ID"
                value={profile.walletId}
                disabled/>
            <DefaultTextField
                className={classes.textField}
                label="User ID"
                value={profile.userId}
                disabled/>

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
