import React from "react";
import Header from "../api/components/Header.tsx";
import NavLinks from "../api/components/NavLinks.tsx";
import AuthForm from "./AuthForm";
import Container from "../api/components/container/Container.tsx";


const EntryPage: React.FC = () => {

    return (
        <Container center>
            <Header />
            <AuthForm/>
            <NavLinks />
        </Container>
    );
};

export default EntryPage;
