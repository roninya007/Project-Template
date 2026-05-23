namespace Api.Models;

/// <summary>
/// Response body returned on a successful login.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// A signed JWT that the caller should include as a Bearer token on subsequent requests.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// UTC timestamp at which the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}
