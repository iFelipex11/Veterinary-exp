using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinary.API.Data;

namespace Veterinary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    [AllowAnonymous]
    [HttpGet("combo")]
    public async Task<ActionResult> GetCombo()
    {
        return Ok(await _context.Countries.OrderBy(x => x.Name).ToListAsync());
    }
}
