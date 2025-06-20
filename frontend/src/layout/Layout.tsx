import React from "react";
import {
    BrowserRouter as Router,
    Navigate,
    Route,
    Routes,
} from "react-router-dom";
import EntryPage from "../screen/EntryPage.tsx";
import DashboardPage from "../screen/DashboardPage.tsx";
import ProfilePage from "../screen/ProfilePage.tsx";
import SmartContractTestPage from "../screen/SmartContractTestPage.tsx";

const isLoggedIn = () => {
    return Boolean(localStorage.getItem("token"));
};

const ProtectedRoute: React.FC<{ element: React.ReactElement }> = ({
    element,
}) => {
    return isLoggedIn() ? element : <Navigate to="/login" replace />;
};

const Layout: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Navigate to="/login" replace />} />
                <Route path="/login" element={<EntryPage />} />
                <Route
                    path="/home"
                    element={<ProtectedRoute element={<DashboardPage />} />}
                />
                <Route
                    path="/smart"
                    element={
                        <ProtectedRoute element={<SmartContractTestPage />} />
                    }
                />
                <Route
                    path="/profile"
                    element={<ProtectedRoute element={<ProfilePage />} />}
                />
            </Routes>
        </Router>
    );
};

export default Layout;
