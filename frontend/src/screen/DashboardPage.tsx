import React from "react";
import Container from "../components/container/Container.tsx";
import { useNavigate } from "react-router-dom";
import SettingsIcon from '@mui/icons-material/Settings';
import DefaultButton from "../components/button/DefaultButton.tsx";
import NavLinks from "../components/NavLinks.tsx";
import SimpleMap from "../components/map/VehicleMap.tsx";

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();

    return (
        <Container>
            <SimpleMap/>
            <NavLinks isLoggedIn={true}/>
        </Container>
    );
};

export default DashboardPage;
