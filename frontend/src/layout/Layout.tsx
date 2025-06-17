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
import SmartContractTestPage from "../screen/SmartContractTestPage.tsx";
import { useAppDispatch } from "../store/Store.ts";
import { fetchUser } from "../store/reducer/user.ts";
import ReservationPage from "../screen/ReservationPage.tsx";
import useApiStates from "../util/useApiStates.ts";
import type { CarTO } from "../api";

const isLoggedIn = () => {
    return Boolean(localStorage.getItem("token"));
};

const ProtectedRoute: React.FC<{ element: React.ReactElement }> = ({
    element,
}) => {
    return isLoggedIn() ? element : <Navigate to="/login" replace />;
};

const ReservationProtectedRoute: React.FC<{
    element: React.ReactElement;
    reservedCar?: CarTO;
}> = ({ element, reservedCar }) => {
    if (!isLoggedIn()) {
        return <Navigate to="/login" replace />;
    }

    return reservedCar ? element : <Navigate to="/home" replace />;
};

const Layout: React.FC = () => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchUser());
    }, [dispatch]);

    const cars = useApiStates("cars");
    const reservedCar = cars.cars?.reservedCar; // optional chaining für Sicherheit

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
                <Route
                    path="/reservation/:carId"
                    element={
                        <ReservationProtectedRoute
                            reservedCar={reservedCar}
                            element={<ReservationPage />}
                        />
                    }
                />
            </Routes>
        </Router>
    );
};

export default Layout;
