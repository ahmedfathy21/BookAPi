using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookApi.DTOs;
using BookApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "User creation failed", errors = result.Errors });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Expiration = DateTime.UtcNow.AddHours(24)
        });
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Expiration = DateTime.UtcNow.AddHours(24)
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
