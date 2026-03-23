using Microsoft.EntityFrameworkCore;
using Veterinary.Shared.Entities;

namespace Veterinary.API.Data;

public class SeedDb(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task SeedDbAsync()
    {
        await _context.Database.MigrateAsync();
        await CheckPetTypesAsync();
        await CheckServiceTypesAsync();
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
}
