using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowYourPostTaxes.Data;

public class Tax
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
    public string CountryName { get; set; }
    public decimal TaxRate { get; set; } 
    
    public Tax(string countryName, decimal taxRate)
    {
        CountryName = countryName;
        TaxRate = taxRate;
    }
}
