using Api.Models;

namespace Api.Services;

/// <summary>
/// Handles credential validation and JWT generation for authentication.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates the supplied credentials and, if correct, returns a signed JWT.
    /// </summary>
    /// <param name="request">Login request containing email and plaintext password.</param>
    /// <returns>
    /// A <see cref="LoginResponse"/> with a token and expiry when credentials are valid;
    /// <c>null</c> when the email is not found or the password does not match.
    /// </returns>
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
