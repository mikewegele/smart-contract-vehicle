import React, {useEffect, useState} from 'react';

const App: React.FC = () => {

  const [message, setMessage] = useState('');

  useEffect(() => {
    fetch('http://localhost:5147/api/hello')
        .then((res) => res.json())
        .then((data) => setMessage(data.message))
        .catch((err) => console.error('API Error:', err));
  }, []);

  return (
          <div>
              <h1>Nachricht vom Backend:</h1>
              <p>{message}</p>
          </div>
  );
}

export default App;
