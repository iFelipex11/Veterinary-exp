using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinary.API.Data;

namespace Veterinary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatesController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    [AllowAnonymous]
    [HttpGet("combo/{countryId:int}")]
    public async Task<ActionResult> GetCombo(int countryId)
    {
        return Ok(await _context.States
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.Name)
            .ToListAsync());
    }
}
