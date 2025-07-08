import React, { useEffect } from "react";
import {
    BrowserRouter as Router,
    Navigate,
    Route,
    Routes,
} from "react-router-dom";
import EntryPage from "../screen/EntryPage.tsx";
import DashboardPage from "../screen/DashboardPage.tsx";
import ProfilePage from "../screen/ProfilePage.tsx";
import { useAppDispatch } from "../store/Store.ts";
import { fetchUser } from "../store/reducer/user.ts";
import ReservationPage from "../screen/ReservationPage.tsx";
import DrivingPage from "../screen/DrivingPage.tsx";

const isLoggedIn = () => {
    return Boolean(localStorage.getItem("token"));
};

const ProtectedRoute: React.FC<{ element: React.ReactElement }> = ({
    element,
}) => {
    return isLoggedIn() ? element : <Navigate to="/login" replace />;
};

const Layout: React.FC = () => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchUser());
    }, [dispatch]);

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
                    path="/profile"
                    element={<ProtectedRoute element={<ProfilePage />} />}
                />
                <Route
                    path="/reservation/:carId"
                    element={<ProtectedRoute element={<ReservationPage />} />}
                />
                <Route
                    path="/driving/:carId"
                    element={<ProtectedRoute element={<DrivingPage />} />}
                />
            </Routes>
        </Router>
    );
};

export default Layout;
