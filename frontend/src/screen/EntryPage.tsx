import React from "react";
import Header from "../components/Header.tsx";
import AuthForm from "./AuthForm";
import Container from "../components/container/Container.tsx";
import UserCookieManager from "../components/cookie/UserCookieManager.tsx";

const EntryPage: React.FC = () => {
    return (
        <Container center>
            <Header />
            <AuthForm />
            <UserCookieManager />
        </Container>
    );
};

export default EntryPage;
