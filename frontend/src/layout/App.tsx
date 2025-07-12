import React from "react";
import { CookiesProvider } from "react-cookie";
import Layout from "./Layout.tsx";
import { Provider } from "react-redux";
import { store } from "../store/Store.ts";
import { Web3Provider } from "../web3/Web3Provider.tsx";

const App: React.FC = () => {
    return (
        <Provider store={store}>
            <CookiesProvider>
                <Web3Provider>
                    <Layout />
                </Web3Provider>
            </CookiesProvider>
        </Provider>
    );
};

export default App;
