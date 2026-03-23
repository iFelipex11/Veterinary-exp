using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinary.API.Data;

namespace Veterinary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    [AllowAnonymous]
    [HttpGet("combo/{stateId:int}")]
    public async Task<ActionResult> GetCombo(int stateId)
    {
        return Ok(await _context.Cities
            .Where(x => x.StateId == stateId)
            .OrderBy(x => x.Name)
            .ToListAsync());
    }
}
