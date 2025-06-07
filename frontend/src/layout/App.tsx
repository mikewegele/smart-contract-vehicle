import React from "react";
import {CookiesProvider} from "react-cookie";
import Layout from "./Layout.tsx";


import { SessionProvider } from "next-auth/react";

const App: React.FC = () => {
    return (
<<<<<<< Updated upstream
        <CookiesProvider>
            <Layout/>
        </CookiesProvider>
=======
        <Provider store={store}>
            <CookiesProvider>
                <SessionProvider>
                    <Layout />
                </SessionProvider>
            </CookiesProvider>
        </Provider>
>>>>>>> Stashed changes
    );
};

export default App;