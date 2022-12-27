using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShippingService.DAL;
using ShippingService.DTO;
using ShippingService.Entities;

namespace ShippingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly shippingdbContext DBContext;
        public ServicesController(shippingdbContext DBContext)
        {
            this.DBContext = DBContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PostRate(Rate rate)
        {
            DBContext.Rates.Add(rate);
            await DBContext.SaveChangesAsync();
            return Ok(new { id = rate.Id });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PostCompany(Company company)
        {
            DBContext.Companies.Add(company);
            await DBContext.SaveChangesAsync();
            return Ok(new { id = company.Id });
        }


        [HttpGet]
        [Route("{originId}/{destId}")]
        public async Task<ActionResult<List<CompanyDTO>>> GetCompany(int originId, int destId)
        {
            var res = await DBContext.Rates
                .Where(r => r.OriginId == originId && r.DestinationId == destId)
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .Include(r => r.Company)
                .Select(r => new CompanyDTO
                {
                    CompanyID = r.CompanyId,
                    Name = r.Company.Name,
                    Rate = r.Price
                })
                .ToListAsync();
            
            if (res == null || res.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return res;
            }
        }
    }
}