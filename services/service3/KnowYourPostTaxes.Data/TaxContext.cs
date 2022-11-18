using Microsoft.EntityFrameworkCore;

namespace KnowYourPostTaxes.Data;

public class TaxContext : DbContext
{
    public DbSet<Tax> Taxes { get; set; }

    public TaxContext(DbContextOptions<TaxContext> options) 
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}
