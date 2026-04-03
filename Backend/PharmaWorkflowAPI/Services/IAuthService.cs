using PharmaWorkflowAPI.DTOs;

namespace PharmaWorkflowAPI.Services;

public interface IAuthService
{
    Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
    string GenerateJwtToken(string username, string role, string fullName);
}
