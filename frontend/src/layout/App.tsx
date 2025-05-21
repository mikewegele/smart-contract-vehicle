import React from "react";
import {CookiesProvider} from "react-cookie";
import Layout from "./Layout.tsx";

const App: React.FC = () => {
    return (
        <CookiesProvider>
            <Layout/>
        </CookiesProvider>
    );
};

export default App;
