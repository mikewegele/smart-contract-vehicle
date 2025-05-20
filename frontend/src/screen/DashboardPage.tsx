import React from "react";
import Container from "../api/components/container/Container.tsx";
import { useNavigate } from "react-router-dom";
import SettingsIcon from '@mui/icons-material/Settings';
import DefaultButton from "../api/components/button/DefaultButton.tsx";

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();

    return (
        <Container center>
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
