using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShippingService.DAL;
using ShippingService.Entities;

namespace ShippingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly shippingdbContext DBContext;
        public CountriesController(shippingdbContext DBContext)
        {
            this.DBContext = DBContext;
        }


        [HttpGet]
        public async Task<ActionResult<List<Country>>> Get()
        {
            var List = await DBContext.Countries.ToListAsync();

            if (List.Count < 0)
            {
                return NotFound();
            }
            else
            {
                return List;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Country>> Post(Country country)
        {
            DBContext.Countries.Add(country);
            await DBContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = country.Id }, country);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<dynamic>> Get(int id)
        {
            var rates = await DBContext.Rates.ToListAsync();
            var countries = await DBContext.Countries.ToListAsync();

            var allowedDestinations = await DBContext.Rates
                .Include(p => p.Origin)
                .Include(p => p.Destination)
                .Where(p => p.OriginId == id)
                .Select(p => new { name = p.Destination.Name, id = p.DestinationId })
                .Distinct()
                .ToListAsync();

            if (allowedDestinations.Count < 0)
            {
                return NotFound();
            }
            else
            {
                return allowedDestinations;
            }
        }
    }
}
