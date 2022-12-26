using System;
using System.Collections.Generic;

#nullable disable

namespace ShippingService.Entities
{
    public partial class Rate
    {
        public int CompanyId { get; set; }
        public string OriginId { get; set; }
        public string DestinationId { get; set; }
        public double Price { get; set; }

        public virtual Company Company { get; set; }
        public virtual Country Destination { get; set; }
        public virtual Country Origin { get; set; }
    }
}
