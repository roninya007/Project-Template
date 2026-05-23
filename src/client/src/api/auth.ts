import type { LoginErrorResponse, LoginRequest, LoginResponse } from '../types/auth';

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export async function loginUser(req: LoginRequest): Promise<LoginResponse> {
  const response = await fetch('/api/v1/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(req),
  });

  if (response.ok) {
    return response.json() as Promise<LoginResponse>;
  }

  if (response.status === 401) {
    const body = (await response.json()) as LoginErrorResponse;
    throw new ApiError(401, body.message);
  }

  throw new ApiError(response.status, 'Login failed. Try again.');
}
