using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowYourPostTaxes.Data;

public class Tax
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
    public string Name { get; set; }
    public float TaxRate { get; set; } 
    
    public Tax(string name, float taxrate)
    {
        Name = name;
        TaxRate = taxrate;
    }
}
