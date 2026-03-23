using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinary.API.Data;

namespace Veterinary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnersController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        return Ok(await _context.Owners.ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> Get(int id)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == id);

        if (owner is null)
        {
            return NotFound();
        }

        return Ok(owner);
    }
}
