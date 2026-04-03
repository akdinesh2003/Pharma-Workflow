using Microsoft.AspNetCore.Mvc;
using PharmaWorkflowAPI.DTOs;
using PharmaWorkflowAPI.Services;

namespace PharmaWorkflowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.AuthenticateAsync(request);

            if (response == null)
            {
                _logger.LogWarning($"Failed login attempt for user: {request.Username}");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation($"User logged in: {request.Username}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
}
