import React, {
    createContext,
    type ReactNode,
    useContext,
    useEffect,
    useRef,
    useState,
} from "react";
import Web3 from "web3";
import CarRentalContract from "../contracts/CarRental.json";
import type { CarRental } from "../../types/CarRental";

export interface IWeb3Context {
    web3: Web3 | null;
    account: string | null;
    contract: CarRental | null;
}

const Web3Context = createContext<IWeb3Context | undefined>(undefined);

export const Web3Provider: React.FC<{ children: ReactNode }> = ({
    children,
}) => {
    const [web3, setWeb3] = useState<Web3 | null>(null);
    const [account, setAccount] = useState<string | null>(null);
    const [contract, setContract] = useState<CarRental | null>(null);

    const hasInitialized = useRef(false);

    useEffect(() => {
        if (hasInitialized.current) return;
        hasInitialized.current = true;

        async function init() {
            try {
                const web3Instance = new Web3(
                    new Web3.providers.HttpProvider("http://127.0.0.1:7545")
                );
                const accounts = await web3Instance.eth.getAccounts();
                setAccount(accounts[0]);
                setWeb3(web3Instance);

                const networkId = await web3Instance.eth.net.getId();
                const deployedNetwork = CarRentalContract.networks[networkId];
                if (!deployedNetwork) {
                    // alert("Smart contract not deployed on the current network");
                    return;
                }
                const contractInstance = new web3Instance.eth.Contract(
                    CarRentalContract.abi,
                    deployedNetwork.address
                ) as unknown as CarRental;
                setContract(contractInstance);
            } catch (error) {
                console.error("MetaMask connection failed:", error);
            }
        }

        init();
    }, []);

    return (
        <Web3Context.Provider value={{ web3, account, contract }}>
            {children}
        </Web3Context.Provider>
    );
};

export const useWeb3 = (): IWeb3Context => {
    const context = useContext(Web3Context);
    if (!context) throw new Error("useWeb3 must be used within Web3Provider");
    return context;
};
