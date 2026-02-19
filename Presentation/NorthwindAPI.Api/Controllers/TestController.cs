using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace NorthwindAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly NorthwindDbContext _context;

    public TestController(NorthwindDbContext context)
    {
        _context = context;
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _context.Customers
            .Select(x => new
            {
                x.CustomerId,
                x.CompanyName,
                x.City
            })
            .Take(5)
            .ToListAsync();

        return Ok(customers);
    }
}
