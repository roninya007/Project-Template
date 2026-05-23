using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

/// <summary>
/// Default implementation of <see cref="IAuthService"/>.
/// Validates credentials against an in-memory user store and issues signed JWTs.
/// </summary>
public sealed class AuthService : IAuthService
{
    // ---------------------------------------------------------------------------
    // In-memory user store (replace with a real database in production).
    // Passwords are stored as BCrypt hashes.
    // ---------------------------------------------------------------------------
    private static readonly IReadOnlyDictionary<string, string> Users =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // BCrypt hash of "P@ssw0rd!" – generated at startup for illustration.
            // In production these rows come from a database.
            ["alice@example.com"] = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
        };

    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initialises a new <see cref="AuthService"/> with access to application configuration.
    /// </summary>
    /// <param name="configuration">Application configuration used to read JWT settings.</param>
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // Look up the stored hash for the given email.
        if (!Users.TryGetValue(request.Email, out var storedHash))
            return Task.FromResult<LoginResponse?>(null);

        // Verify the plaintext password against the stored BCrypt hash.
        if (!BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
            return Task.FromResult<LoginResponse?>(null);

        var response = GenerateToken(request.Email);
        return Task.FromResult<LoginResponse?>(response);
    }

    // ---------------------------------------------------------------------------
    // Private helpers
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Generates a signed JWT for the specified subject (email).
    /// </summary>
    /// <param name="email">The authenticated user's email address, used as the token subject.</param>
    /// <returns>A <see cref="LoginResponse"/> containing the serialised token and its expiry.</returns>
    private LoginResponse GenerateToken(string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey   = jwtSettings["Key"]    ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer      = jwtSettings["Issuer"]  ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience    = jwtSettings["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt   = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
