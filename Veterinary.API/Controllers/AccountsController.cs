using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
public class AccountsController(IUserHelper userHelper, IMailHelper mailHelper, IConfiguration configuration) : ControllerBase
{
    private readonly IUserHelper _userHelper = userHelper;
    private readonly IMailHelper _mailHelper = mailHelper;
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
            CityId = model.CityId,
            PhoneNumber = model.PhoneNumber,
            Photo = model.Photo,
            UserType = model.UserType
        };

        var result = await _userHelper.AddUserAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
        var emailSent = await SendConfirmationEmailAsync(user);

        if (emailSent)
        {
            return Ok("Tu usuario fue creado. Revisa tu correo para confirmar la cuenta antes de iniciar sesion.");
        }

        return Ok("Tu usuario fue creado, pero no se pudo enviar el correo de confirmacion. Configura SMTP y usa reenviar confirmacion.");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user is null)
        {
            return BadRequest("Email o contrasena incorrectos.");
        }

        if (!await _userHelper.IsEmailConfirmedAsync(user))
        {
            return BadRequest("Debes confirmar tu correo antes de iniciar sesion.");
        }

        var result = await _userHelper.LoginAsync(model);
        if (!result.Succeeded)
        {
            return BadRequest("Email o contrasena incorrectos.");
        }

        return Ok(BuildToken(user));
    }

    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var user = await _userHelper.GetUserByIdAsync(userId);
        if (user is null)
        {
            return BadRequest("Usuario no encontrado.");
        }

        var decodedToken = DecodeToken(token);
        var result = await _userHelper.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        return Ok("Tu correo fue confirmado correctamente. Ya puedes iniciar sesion.");
    }

    [AllowAnonymous]
    [HttpPost("ResendConfirmation")]
    public async Task<ActionResult> ResendConfirmation([FromBody] EmailDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user is null)
        {
            return BadRequest("No existe un usuario con ese correo.");
        }

        if (await _userHelper.IsEmailConfirmedAsync(user))
        {
            return Ok("Tu correo ya estaba confirmado. Ya puedes iniciar sesion.");
        }

        var emailSent = await SendConfirmationEmailAsync(user);
        if (!emailSent)
        {
            return BadRequest("No fue posible enviar el correo de confirmacion. Revisa la configuracion SMTP.");
        }

        return Ok("Te enviamos nuevamente el correo de confirmacion.");
    }

    [AllowAnonymous]
    [HttpPost("RecoverPassword")]
    public async Task<ActionResult> RecoverPassword([FromBody] EmailDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user is null)
        {
            return BadRequest("No existe un usuario con ese correo.");
        }

        var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
        var encodedToken = EncodeToken(token);
        var url = $"{_configuration["frontendUrl"]}/resetpassword?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(encodedToken)}";
        var body = $$"""
            <h3>Recuperacion de contrasena</h3>
            <p>Hola {{user.FullName}},</p>
            <p>Haz clic en el siguiente enlace para restablecer tu contrasena:</p>
            <p><a href="{{url}}">Restablecer contrasena</a></p>
            """;

        var emailSent = await _mailHelper.SendMailAsync(user.FullName, user.Email!, "Veterinary - Recuperar contrasena", body);
        if (!emailSent)
        {
            return BadRequest("No fue posible enviar el correo de recuperacion. Revisa la configuracion SMTP.");
        }

        return Ok("Te enviamos un correo para recuperar tu contrasena.");
    }

    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user is null)
        {
            return BadRequest("No existe un usuario con ese correo.");
        }

        var decodedToken = DecodeToken(model.Token);
        var result = await _userHelper.ResetPasswordAsync(user, decodedToken, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        return Ok("Tu contrasena fue restablecida correctamente.");
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Get()
    {
        var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Put([FromBody] User user)
    {
        var currentUser = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (currentUser is null)
        {
            return NotFound();
        }

        currentUser.Document = user.Document;
        currentUser.FirstName = user.FirstName;
        currentUser.LastName = user.LastName;
        currentUser.Address = user.Address;
        currentUser.PhoneNumber = user.PhoneNumber;
        currentUser.Photo = user.Photo;
        currentUser.CityId = user.CityId;

        var result = await _userHelper.UpdateUserAsync(currentUser);
        if (result.Succeeded)
        {
            return Ok(BuildToken(currentUser));
        }

        return BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (user is null)
        {
            return NotFound();
        }

        var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        return NoContent();
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
            new("Photo", user.Photo ?? string.Empty),
            new("CityId", user.CityId.ToString())
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

    private async Task<bool> SendConfirmationEmailAsync(User user)
    {
        var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = EncodeToken(token);
        var url = $"{_configuration["frontendUrl"]}/confirmemail?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(encodedToken)}";
        var body = $$"""
            <h3>Confirmacion de cuenta</h3>
            <p>Hola {{user.FullName}},</p>
            <p>Gracias por registrarte en Veterinary.</p>
            <p>Para activar tu cuenta, haz clic en el siguiente enlace:</p>
            <p><a href="{{url}}">Confirmar correo</a></p>
            """;

        return await _mailHelper.SendMailAsync(user.FullName, user.Email!, "Veterinary - Confirmar correo", body);
    }

    private static string EncodeToken(string token)
    {
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    private static string DecodeToken(string token)
    {
        return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
    }
}
