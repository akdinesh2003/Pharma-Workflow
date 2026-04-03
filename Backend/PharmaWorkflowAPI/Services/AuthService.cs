using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PharmaWorkflowAPI.Data;
using PharmaWorkflowAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PharmaWorkflowAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
            return null;

        // Simple password verification (in production, use proper hashing like BCrypt)
        // For demo: admin/Admin@123, manager/Manager@123, user/User@123
        bool isValidPassword = VerifyPassword(request.Password, user.Role);
        
        if (!isValidPassword)
            return null;

        var token = GenerateJwtToken(user.Username, user.Role, user.FullName);
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            FullName = user.FullName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    public string GenerateJwtToken(string username, string role, string fullName)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("FullName", fullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string role)
    {
        // Simple demo password verification
        return role switch
        {
            "Admin" => password == "Admin@123",
            "Manager" => password == "Manager@123",
            "User" => password == "User@123",
            _ => false
        };
    }
}
