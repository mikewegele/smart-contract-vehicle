import React from 'react';
import {BrowserRouter as Router, Routes, Route, Navigate} from 'react-router-dom';
import LoginScreen from "../screens/LoginScreen.tsx";
import DashboardScreen from "../screens/DashboardScreen.tsx";


const App: React.FC = () => {

  return (
      <Router>
        <Routes>
          <Route path="/login" element={<LoginScreen />} />
          <Route
              path="/dashboard" element={<DashboardScreen />}/>
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </Router>
  );
}

export default App;
