using System;
using System.Collections.Generic;

#nullable disable

namespace ShippingService.Entities
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
