namespace ShippingService
{
    public class CountriesRequest
    {
        public int id { get; set; }
        public List<string> countries { get; set; }

        public OrderStatus status { get; set; }
    }
    public enum OrderStatus
    {
        IN_PROGRESS,
        COMPLETED,
        REJECTED
    }
}
