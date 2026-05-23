import { type FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ApiError, loginUser } from '../api/auth';
import styles from './LoginPage.module.css';

export default function LoginPage() {
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const response = await loginUser({ email, password });
      localStorage.setItem('token', response.token);
      localStorage.setItem('tokenExpiresAt', response.expiresAt);
      void navigate('/dashboard');
    } catch (err) {
      if (err instanceof ApiError && err.status === 401) {
        setError(err.message);
      } else {
        setError('Login failed. Try again.');
      }
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <main className={styles.page}>
      <div className={styles.card}>
        <h1 className={styles.title}>Sign in</h1>

        {error !== null && (
          <p role="alert" className={styles.error}>
            {error}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className={styles.field}>
            <label htmlFor="email" className={styles.label}>
              Email
            </label>
            <input
              id="email"
              type="email"
              className={styles.input}
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="you@example.com"
              required
              autoComplete="email"
              disabled={isLoading}
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="password" className={styles.label}>
              Password
            </label>
            <input
              id="password"
              type="password"
              className={styles.input}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              required
              minLength={1}
              autoComplete="current-password"
              disabled={isLoading}
            />
          </div>

          <button type="submit" className={styles.button} disabled={isLoading}>
            {isLoading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
      </div>
    </main>
  );
}
