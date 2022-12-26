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
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<dynamic>> Get(string id)
        {
            var rates = await DBContext.Rates.ToListAsync();
            var countries = await DBContext.Countries.ToListAsync();

            var allowedDestinations = from r in rates
                                      where r.OriginId == id
                                      select r;

            var result = (from a in allowedDestinations
                         join c in countries on a.DestinationId equals c.Code
                         select new { name = c.Name, fullName = c.FullName, code = c.Code }).Distinct();

            var response = result.ToList();
            if (response.Count < 0)
            {
                return NotFound();
            }
            else
            {
                return response;
            }
        }
    }
}
