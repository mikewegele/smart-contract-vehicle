import React from "react";
import Header from "../components/Header.tsx";
import NavLinks from "../components/NavLinks.tsx";
import AuthForm from "./AuthForm";
import Container from "../components/container/Container.tsx";
import UserCookieManager from "../components/cookie/UserCookieManager.tsx";


const EntryPage: React.FC = () => {

    return (
        <Container center>
            <Header/>
            <AuthForm/>
            <UserCookieManager/>
            <NavLinks isLoggedIn={false}/>
        </Container>
    );
};

export default EntryPage;
