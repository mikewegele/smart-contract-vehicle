import React from "react";
import { CookiesProvider } from "react-cookie";
import Layout from "./Layout.tsx";
import { Provider } from "react-redux";
import { store } from "../store/Store.ts";

const App: React.FC = () => {
    return (
        <Provider store={store}>
            <CookiesProvider>
                <Layout />
            </CookiesProvider>
        </Provider>
    );
};

export default App;
