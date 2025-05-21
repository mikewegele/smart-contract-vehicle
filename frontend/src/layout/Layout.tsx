import React from "react";
import {BrowserRouter as Router, Navigate, Route, Routes} from "react-router-dom";
import EntryPage from "../screen/EntryPage.tsx";
import DashboardPage from "../screen/DashboardPage.tsx";
import ProfilePage from "../screen/ProfilePage.tsx";

const Layout: React.FC = () => {

    return (
        <Router>
            <Routes>
                <Route path="/" element={<Navigate to="/login" replace/>}/>
                <Route path="/login" element={<EntryPage/>}/>
                <Route path="/home" element={<DashboardPage/>}/>
                <Route path="/profile" element={<ProfilePage/>}/>
            </Routes>
        </Router>
    );
}

export default Layout;