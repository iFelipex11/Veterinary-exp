using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Veterinary.API.Helpers;
using Veterinary.Shared.DTOs;
using Veterinary.Shared.Entities;

namespace Veterinary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IUserHelper userHelper, IConfiguration configuration) : ControllerBase
{
    private readonly IUserHelper _userHelper = userHelper;
    private readonly IConfiguration _configuration = configuration;

    [AllowAnonymous]
    [HttpPost("CreateUser")]
    public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
    {
        var user = new User
        {
            Document = model.Document,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Photo = model.Photo,
            UserType = model.UserType
        };

        var result = await _userHelper.AddUserAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
            return Ok(BuildToken(user));
        }

        return BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO model)
    {
        var result = await _userHelper.LoginAsync(model);
        if (!result.Succeeded)
        {
            return BadRequest("Email o contrasena incorrectos.");
        }

        var user = await _userHelper.GetUserAsync(model.Email);
        if (user is null)
        {
            return BadRequest("Usuario no encontrado.");
        }

        return Ok(BuildToken(user));
    }

    private TokenDTO BuildToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email!),
            new(ClaimTypes.Role, user.UserType.ToString()),
            new("Document", user.Document),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName),
            new("Address", user.Address),
            new("Photo", user.Photo ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}
