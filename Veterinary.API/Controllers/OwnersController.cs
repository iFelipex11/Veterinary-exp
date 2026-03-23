using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinary.API.Data;
using Veterinary.Shared.Entities;

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

    [HttpPost]
    public async Task<ActionResult> Post(Owner owner)
    {
        var exists = await _context.Owners.AnyAsync(x => x.Document == owner.Document);

        if (exists)
        {
            return BadRequest("An owner with the same document already exists.");
        }

        _context.Add(owner);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = owner.Id }, owner);
    }

    [HttpPut]
    public async Task<ActionResult> Put(Owner owner)
    {
        var currentOwner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == owner.Id);
        if (currentOwner is null)
        {
            return NotFound();
        }

        var exists = await _context.Owners.AnyAsync(x => x.Document == owner.Document && x.Id != owner.Id);
        if (exists)
        {
            return BadRequest("An owner with the same document already exists.");
        }

        _context.Entry(currentOwner).CurrentValues.SetValues(owner);
        await _context.SaveChangesAsync();
        return Ok(currentOwner);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var affectedRows = await _context.Owners
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}
