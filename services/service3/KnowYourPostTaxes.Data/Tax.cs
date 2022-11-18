using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowYourPostTaxes.Data;

public class Tax
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
    public string Name { get; set; }
    public decimal(4, 3) TaxRate { get; set; } 
    
    public Tax(string name, decimal(4, 3) taxrate)
    {
        Name = name;
        TaxRate = taxrate;
    }
}
