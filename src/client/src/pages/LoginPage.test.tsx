import { act, render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { ApiError, loginUser } from '../api/auth';
import LoginPage from './LoginPage';

jest.mock('../api/auth', () => ({
  ...jest.requireActual('../api/auth'),
  loginUser: jest.fn(),
}));

const mockNavigate = jest.fn();
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

const mockLoginUser = loginUser as jest.MockedFunction<typeof loginUser>;

function renderLoginPage() {
  return render(
    <MemoryRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
      <LoginPage />
    </MemoryRouter>
  );
}

const CREDENTIALS = { email: 'user@example.com', password: 'P@ssw0rd!' };
const SUCCESS = { token: 'jwt-abc', expiresAt: '2026-12-31T00:00:00Z' };

describe('LoginPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    localStorage.clear();
  });

  // ── Rendering ──────────────────────────────────────────────────────────────

  describe('initial render', () => {
    it('renders email input', () => {
      renderLoginPage();
      expect(screen.getByLabelText('Email')).toBeInTheDocument();
    });

    it('renders password input', () => {
      renderLoginPage();
      expect(screen.getByLabelText('Password')).toBeInTheDocument();
    });

    it('renders submit button with "Sign in" label', () => {
      renderLoginPage();
      expect(screen.getByRole('button', { name: 'Sign in' })).toBeInTheDocument();
    });

    it('shows no error alert on initial render', () => {
      renderLoginPage();
      expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    });
  });

  // ── Successful login ───────────────────────────────────────────────────────

  describe('successful login', () => {
    beforeEach(() => {
      mockLoginUser.mockResolvedValue(SUCCESS);
    });

    async function submitForm() {
      const user = userEvent.setup();
      renderLoginPage();
      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));
    }

    it('calls loginUser with the typed credentials', async () => {
      await submitForm();
      expect(mockLoginUser).toHaveBeenCalledWith(CREDENTIALS);
      expect(mockLoginUser).toHaveBeenCalledTimes(1);
    });

    it('stores token in localStorage', async () => {
      await submitForm();
      await waitFor(() => expect(localStorage.getItem('token')).toBe(SUCCESS.token));
    });

    it('stores token expiry in localStorage', async () => {
      await submitForm();
      await waitFor(() => expect(localStorage.getItem('tokenExpiresAt')).toBe(SUCCESS.expiresAt));
    });

    it('navigates to /dashboard', async () => {
      await submitForm();
      await waitFor(() => expect(mockNavigate).toHaveBeenCalledWith('/dashboard'));
    });
  });

  // ── Loading state ──────────────────────────────────────────────────────────

  describe('loading state', () => {
    function makeDelayedLogin() {
      let resolve!: (v: typeof SUCCESS) => void;
      const promise = new Promise<typeof SUCCESS>(r => { resolve = r; });
      mockLoginUser.mockReturnValue(promise);
      return { resolve };
    }

    it('disables submit button while request is in flight', async () => {
      const { resolve } = makeDelayedLogin();
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(screen.getByRole('button')).toBeDisabled();

      resolve(SUCCESS);
      await waitFor(() => expect(screen.getByRole('button')).not.toBeDisabled());
    });

    it('shows "Signing in…" on the button while loading', async () => {
      const { resolve } = makeDelayedLogin();
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(screen.getByRole('button')).toHaveTextContent('Signing in');

      await act(async () => { resolve(SUCCESS); });
    });

    it('disables email input while loading', async () => {
      const { resolve } = makeDelayedLogin();
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(screen.getByLabelText('Email')).toBeDisabled();

      await act(async () => { resolve(SUCCESS); });
    });

    it('disables password input while loading', async () => {
      const { resolve } = makeDelayedLogin();
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(screen.getByLabelText('Password')).toBeDisabled();

      await act(async () => { resolve(SUCCESS); });
    });
  });

  // ── 401 errors ─────────────────────────────────────────────────────────────

  describe('401 Unauthorized', () => {
    const error401 = new ApiError(401, 'Invalid email or password.');

    it('shows the server error message', async () => {
      mockLoginUser.mockRejectedValue(error401);
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(await screen.findByRole('alert')).toHaveTextContent('Invalid email or password.');
    });

    it('re-enables the submit button after error', async () => {
      mockLoginUser.mockRejectedValue(error401);
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      await screen.findByRole('alert');
      expect(screen.getByRole('button', { name: 'Sign in' })).not.toBeDisabled();
    });

    it('clears previous error when form is resubmitted', async () => {
      mockLoginUser
        .mockRejectedValueOnce(error401)
        .mockResolvedValueOnce(SUCCESS);

      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));
      await screen.findByRole('alert');

      await user.click(screen.getByRole('button', { name: 'Sign in' }));
      expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    });

    it('does not write to localStorage on 401', async () => {
      mockLoginUser.mockRejectedValue(error401);
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      await screen.findByRole('alert');
      expect(localStorage.getItem('token')).toBeNull();
    });
  });

  // ── Non-401 errors ─────────────────────────────────────────────────────────

  describe('non-401 errors', () => {
    it('shows generic message on 500 error', async () => {
      mockLoginUser.mockRejectedValue(new ApiError(500, 'Login failed. Try again.'));
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(await screen.findByRole('alert')).toHaveTextContent('Login failed. Try again.');
    });

    it('shows generic message on network failure', async () => {
      mockLoginUser.mockRejectedValue(new Error('Network error'));
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      expect(await screen.findByRole('alert')).toHaveTextContent('Login failed. Try again.');
    });

    it('re-enables submit button after non-401 error', async () => {
      mockLoginUser.mockRejectedValue(new ApiError(500, 'Login failed. Try again.'));
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      await screen.findByRole('alert');
      expect(screen.getByRole('button', { name: 'Sign in' })).not.toBeDisabled();
    });

    it('does not navigate on error', async () => {
      mockLoginUser.mockRejectedValue(new ApiError(500, 'Login failed. Try again.'));
      const user = userEvent.setup();
      renderLoginPage();

      await user.type(screen.getByLabelText('Email'), CREDENTIALS.email);
      await user.type(screen.getByLabelText('Password'), CREDENTIALS.password);
      await user.click(screen.getByRole('button', { name: 'Sign in' }));

      await screen.findByRole('alert');
      expect(mockNavigate).not.toHaveBeenCalled();
    });
  });
});
