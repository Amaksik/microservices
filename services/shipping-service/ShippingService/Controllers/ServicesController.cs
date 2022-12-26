using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using ShippingService.DAL;
using ShippingService.DTO;
using ShippingService.Entities;
using System.Configuration;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult<List<CompanyDTO>>> GetCompany(string code)
        {
            var companies = await DBContext.Companies.ToListAsync();
            var rates = await DBContext.Rates.Where(p => p.OriginId == code.ToUpper()).ToListAsync();

            var result = CreateRatesDTO(rates, companies, code);

            if (result.Count < 0)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }

        private List<CompanyDTO> CreateRatesDTO(List<Rate> rates, List<Company> companies, string code)
        {
            var result = new List<CompanyDTO>();

            foreach(Company company in companies)
            {
                var newCompany = new CompanyDTO
                {
                    CompanyID= company.Id,
                    Name = company.Name,
                    Rates = new List<RateDTO>()
                };
                result.Add(newCompany);
            }
            foreach (Rate rate in rates)
            {
                var element = result.Where(p=>p.CompanyID==rate.CompanyId).Single();
                
                element.Rates = rates.Where(p => p.CompanyId == element.CompanyID)
                    .Select(e=>new RateDTO
                    {
                        Rate = e.Price,
                        Origin= e.OriginId,
                        Destination= e.DestinationId
                    

                    }).ToList();
            }
            result.RemoveAll(p => p.Rates.Count == 0);

            return result;


        }
    }
}