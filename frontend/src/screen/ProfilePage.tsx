import React from "react";
import Container from "../components/container/Container.tsx";
import DefaultTextField from "../components/textfield/DefaultTextField.tsx";

const PROFILE = {
    firstname: "Eyal",
    lastname: "Johnson",
    email: "Eyal@gmail.com",
}

const ProfilePage: React.FC = () => {

    return (
        <Container>
            <DefaultTextField label={"First Name"} value={PROFILE.firstname}/>
            <DefaultTextField label={"Last Name"} value={PROFILE.lastname}/>
        </Container>
    )
}

export default ProfilePage;
