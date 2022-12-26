using System;
using System.Collections.Generic;

#nullable disable

namespace ShippingService.Entities
{
    public partial class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Iso3 { get; set; }
        public string Number { get; set; }
        public string ContinentCode { get; set; }

        public virtual Continent ContinentCodeNavigation { get; set; }
    }
}
