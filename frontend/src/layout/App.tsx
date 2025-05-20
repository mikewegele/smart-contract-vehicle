import React from "react";
import {BrowserRouter as Router, Routes, Route, Navigate} from "react-router-dom";
import EntryPage from "../screen/EntryPage.tsx";
import DashboardPage from "../screen/DashboardPage.tsx";

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Navigate to="/login" replace />} />
                <Route path="/login" element={<EntryPage />} />
                <Route path="/home" element={<DashboardPage />} />
            </Routes>
        </Router>
    );
};

export default App;
