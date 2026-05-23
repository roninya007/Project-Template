using Api.Controllers;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_authService.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "P@ssw0rd!" };
        var expected = new LoginResponse { Token = "jwt-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
        _authService.Setup(s => s.LoginAsync(request)).ReturnsAsync(expected);

        var result = await _sut.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "wrong" };
        _authService.Setup(s => s.LoginAsync(request)).ReturnsAsync((LoginResponse?)null);

        var result = await _sut.Login(request);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ResponseBodyContainsMessage()
    {
        var request = new LoginRequest { Email = "nobody@example.com", Password = "bad" };
        _authService.Setup(s => s.LoginAsync(request)).ReturnsAsync((LoginResponse?)null);

        var result = await _sut.Login(request);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = unauthorized.Value!.ToString();
        Assert.Contains("Invalid email or password", body);
    }

    [Fact]
    public async Task Login_ServiceReturnsToken_TokenSurfacedInResponse()
    {
        const string token = "eyJhbGciOiJIUzI1NiJ9.payload.sig";
        var request = new LoginRequest { Email = "user@example.com", Password = "P@ssw0rd!" };
        _authService.Setup(s => s.LoginAsync(request))
                    .ReturnsAsync(new LoginResponse { Token = token, ExpiresAt = DateTime.UtcNow.AddHours(1) });

        var result = await _sut.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.Equal(token, response.Token);
    }

    [Fact]
    public async Task Login_CallsServiceExactlyOnce()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "P@ssw0rd!" };
        _authService.Setup(s => s.LoginAsync(request)).ReturnsAsync((LoginResponse?)null);

        await _sut.Login(request);

        _authService.Verify(s => s.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_ServiceReturnsExpiry_ExpirySurfacedInResponse()
    {
        var expiry = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var request = new LoginRequest { Email = "user@example.com", Password = "P@ssw0rd!" };
        _authService.Setup(s => s.LoginAsync(request))
                    .ReturnsAsync(new LoginResponse { Token = "t", ExpiresAt = expiry });

        var result = await _sut.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.Equal(expiry, response.ExpiresAt);
    }
}
