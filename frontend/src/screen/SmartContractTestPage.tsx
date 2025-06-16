import React, { useEffect, useState } from "react";
import Container from "../components/container/Container.tsx";
import NavLinks from "../components/NavLinks.tsx";
import { useWeb3 } from "../web3/Web3Provider.tsx";
import { Typography } from "@mui/material";

const SmartContractTestPage: React.FC = () => {
    const { web3, account, contract } = useWeb3();

    const [message, setMessage] = useState("");

    useEffect(() => {
        const fetchMessage = async () => {
            if (!web3) {
                setMessage("Loading Web3...");
                return;
            }
            if (!account) {
                setMessage("Please connect your wallet");
                return;
            }
            if (!contract) {
                setMessage("Contract not loaded on this network");
                return;
            }
            try {
                const helloWorld = await contract.methods.helloWorld().call();
                setMessage(helloWorld);
            } catch (error) {
                console.error(error);
                setMessage("Error fetching message from contract.");
            }
        };
        fetchMessage();
    }, [web3, account, contract]);

    return (
        <Container>
            <NavLinks isLoggedIn={true} />
            <Typography>{message}</Typography>
        </Container>
    );
};

export default SmartContractTestPage;
