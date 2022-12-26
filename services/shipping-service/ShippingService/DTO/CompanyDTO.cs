
namespace ShippingService.DTO
{
    public class CompanyDTO
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public List<RateDTO> Rates { get; set; }

    }
}