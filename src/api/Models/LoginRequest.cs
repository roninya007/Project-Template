using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Request body for the login endpoint.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    /// <example>user@example.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The user's plaintext password.
    /// </summary>
    /// <example>P@ssw0rd!</example>
    [Required]
    public string Password { get; init; } = string.Empty;
}
