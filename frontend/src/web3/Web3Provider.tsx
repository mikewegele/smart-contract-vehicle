// src/web3/Web3Provider.tsx
import React, {
    createContext,
    ReactNode,
    useContext,
    useEffect,
    useState,
} from "react";
import Web3 from "web3";
import CarRentalContract from "../contracts/CarRental.json";

interface IWeb3Context {
    web3: Web3 | null;
    account: string | null;
    contract: any | null;
}

const Web3Context = createContext<IWeb3Context | undefined>(undefined);

export const Web3Provider: React.FC<{ children: ReactNode }> = ({
    children,
}) => {
    const [web3, setWeb3] = useState<Web3 | null>(null);
    const [account, setAccount] = useState<string | null>(null);
    const [contract, setContract] = useState<any | null>(null);

    useEffect(() => {
        async function init() {
            if (!(window as any).ethereum) {
                return;
            }

            try {
                const web3Instance = new Web3((window as any).ethereum);
                await (window as any).ethereum.request({
                    method: "eth_requestAccounts",
                });
                const accounts = await web3Instance.eth.getAccounts();
                setAccount(accounts[0]);
                setWeb3(web3Instance);

                const networkId = await web3Instance.eth.net.getId();
                const deployedNetwork = CarRentalContract.networks[networkId];
                if (!deployedNetwork) {
                    return;
                }

                const contractInstance = new web3Instance.eth.Contract(
                    CarRentalContract.abi,
                    deployedNetwork.address
                );
                setContract(contractInstance);
            } catch {
                return;
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
