import React, { useState } from 'react';
import styles from './EntryPage.module.css';

const EntryPage: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!email.includes('@')) { 
      setError('Email must contain @');
      return;
    }
    if (!isLogin && username.length < 3) {
      setError('Username must be at least 3 characters');
      return;
    }
    if (password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setError('');
    if (isLogin) {
      console.log('Logging in with:', { email, password });
    } else {
      console.log('Signing up with:', { email, username, password });
    }
  };

  return (
    <div className={styles.container}>
    <h1 className={styles.mainTitle}>Smart Car Rental</h1>

    <div className={styles.navLinks}>
        <span>Our Cars</span>
        <span>About Us</span>
        <span>Locations</span>
    </div>

      <div className={styles.card}>
        <h2 className={styles.title}>{isLogin ? 'Log In' : 'Sign Up'}</h2>

        <form onSubmit={handleSubmit} className={styles.form}>
          <label>Email</label>
          <input
            className={styles.input}
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
          />

          {!isLogin && (
            <>
              <label>Username</label>
              <input
                className={styles.input}
                type="text"
                value={username}
                onChange={e => setUsername(e.target.value)}
                required
              />
            </>
          )}

          <label>Password</label>
          <input
            className={styles.input}
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
          />

          {error && <p className={styles.error}>{error}</p>}

          <button type="submit" className={styles.button}>
            {isLogin ? 'Log In' : 'Sign Up'}
          </button>
        </form>

        <p className={styles.toggle}>
          {isLogin ? "Don't have an account?" : 'Already have an account?'}{' '}
          <button className={styles.link} onClick={() => setIsLogin(!isLogin)}>
            {isLogin ? 'Sign Up' : 'Log In'}
          </button>
        </p>
      </div>
    </div>
  );
};

export default EntryPage;