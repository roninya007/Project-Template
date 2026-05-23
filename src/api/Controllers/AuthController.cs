using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Handles authentication operations such as user login.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialises a new <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">Service responsible for credential validation and token issuance.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns a signed JWT on success.
    /// </summary>
    /// <param name="request">Login credentials (email + password).</param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description>200 OK – credentials valid; body contains <see cref="LoginResponse"/>.</description></item>
    ///   <item><description>400 Bad Request – request body fails validation.</description></item>
    ///   <item><description>401 Unauthorized – email not found or password incorrect.</description></item>
    /// </list>
    /// </returns>
    /// <response code="200">Login successful; JWT returned.</response>
    /// <response code="400">Request body is invalid.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(response);
    }
}
