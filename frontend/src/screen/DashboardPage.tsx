import React from "react";
import Container from "../components/container/Container.tsx";
import { useNavigate } from "react-router-dom";
import SettingsIcon from '@mui/icons-material/Settings';
import DefaultButton from "../components/button/DefaultButton.tsx";

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();

    return (
        <Container>
            <VehicleMap/>
            <DefaultButton
                variant="contained"
                color="primary"
                startIcon={<SettingsIcon />}
                onClick={() => navigate("/profile")}>
                Profile
            </DefaultButton>
        </Container>
    );
};

export default DashboardPage;
