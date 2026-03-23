using Microsoft.EntityFrameworkCore;
using Veterinary.API.Helpers;
using Veterinary.Shared.Entities;
using Veterinary.Shared.Enums;

namespace Veterinary.API.Data;

public class SeedDb(DataContext context, IUserHelper userHelper)
{
    private readonly DataContext _context = context;
    private readonly IUserHelper _userHelper = userHelper;

    public async Task SeedDbAsync()
    {
        await _context.Database.MigrateAsync();
        await CheckPetTypesAsync();
        await CheckServiceTypesAsync();
        await CheckRolesAsync();
        await CheckUserAsync("1", "OAP", "OAP", "oap@yopmail.com", "CR 78 9687", UserType.Admin);
    }

    private async Task CheckPetTypesAsync()
    {
        if (_context.PetTypes.Any())
        {
            return;
        }

        _context.PetTypes.Add(new PetType { Name = "Dog" });
        _context.PetTypes.Add(new PetType { Name = "Cat" });
        _context.PetTypes.Add(new PetType { Name = "Bird" });
        await _context.SaveChangesAsync();
    }

    private async Task CheckServiceTypesAsync()
    {
        if (_context.ServiceTypes.Any())
        {
            return;
        }

        _context.ServiceTypes.Add(new ServiceType { Name = "Bath" });
        _context.ServiceTypes.Add(new ServiceType { Name = "Vaccination" });
        _context.ServiceTypes.Add(new ServiceType { Name = "Control" });
        await _context.SaveChangesAsync();
    }

    private async Task CheckRolesAsync()
    {
        await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
        await _userHelper.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(
        string document,
        string firstName,
        string lastName,
        string email,
        string address,
        UserType userType)
    {
        var user = await _userHelper.GetUserAsync(email);
        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            Document = document,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Address = address,
            UserName = email,
            UserType = userType
        };

        var result = await _userHelper.AddUserAsync(user, "123456");
        if (result.Succeeded)
        {
            await _userHelper.AddUserToRoleAsync(user, userType.ToString());
        }

        return user;
    }
}
