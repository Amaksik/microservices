using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ShippingService.Entities
{
    public partial class Rate
    {
        [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Price { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey("Origin")]
        public int OriginId { get; set; }
        public virtual Country Origin { get; set; }

        [ForeignKey("Destination")]
        public int DestinationId { get; set; }
        public virtual Country Destination { get; set; }
        
    }
}
