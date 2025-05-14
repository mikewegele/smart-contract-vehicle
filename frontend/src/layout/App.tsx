import React, {useEffect, useState} from 'react';
import {BrowserRouter as Router, Routes, Route, Navigate} from 'react-router-dom';
import LoginScreen from "../screens/LoginScreen.tsx";
import DashboardScreen from "../screens/DashboardScreen.tsx";


const App: React.FC = () => {

  const [message, setMessage] = useState('');

  useEffect(() => {
    fetch('http://localhost:5147/api/hello')
        .then((res) => res.json())
        .then((data) => setMessage(data.message))
        .catch((err) => console.error('API Error:', err));
  }, []);

  return (
      <>
          <Router>
              <Routes>
                  <Route path="/login" element={<LoginScreen />} />
                  <Route
                      path="/dashboard" element={<DashboardScreen />}/>
                  <Route path="*" element={<Navigate to="/login" replace />} />
              </Routes>
          </Router>
          <div>
              <h1>Nachricht vom Backend:</h1>
              <p>{message}</p>
          </div>
      </>
  );
}

export default App;
