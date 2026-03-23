using Microsoft.AspNetCore.Identity;
using Veterinary.Shared.DTOs;
using Veterinary.Shared.Entities;

namespace Veterinary.API.Helpers;

public interface IUserHelper
{
    Task<User?> GetUserAsync(string email);
    Task<User?> GetUserAsync(Guid userId);
    Task<User?> GetUserByIdAsync(string userId);
    Task<IdentityResult> AddUserAsync(User user, string password);
    Task CheckRoleAsync(string roleName);
    Task AddUserToRoleAsync(User user, string roleName);
    Task<bool> IsUserInRoleAsync(User user, string roleName);
    Task<bool> IsEmailConfirmedAsync(User user);
    Task<SignInResult> LoginAsync(LoginDTO model);
    Task LogoutAsync();
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IdentityResult> UpdateUserAsync(User user);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
}
