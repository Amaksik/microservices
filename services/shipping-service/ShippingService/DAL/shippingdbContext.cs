using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ShippingService.Entities;

#nullable disable

namespace ShippingService.DAL
{
    public partial class shippingdbContext : DbContext
    {
        public shippingdbContext()
        {
        }

        public shippingdbContext(DbContextOptions<shippingdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Continent> Continents { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseMySQL("server=localhost;port=3307;user=root;password=root;database=shippingdb;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PRIMARY");

                entity.Property(e => e.Code)
                    .HasMaxLength(2)
                    .IsFixedLength(true)
                    .HasComment("Continent code");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ContinentCode, "continent_code");

                entity.Property(e => e.Code)
                    .HasMaxLength(2)
                    .HasColumnName("code")
                    .IsFixedLength(true)
                    .HasComment("Two-letter country code (ISO 3166-1 alpha-2)");

                entity.Property(e => e.ContinentCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("continent_code")
                    .IsFixedLength(true);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("full_name")
                    .HasComment("Full English country name");

                entity.Property(e => e.Iso3)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("iso3")
                    .IsFixedLength(true)
                    .HasComment("Three-letter country code (ISO 3166-1 alpha-3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name")
                    .HasComment("English country name");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("number")
                    .IsFixedLength(true)
                    .HasComment("Three-digit country number (ISO 3166-1 numeric)");

                entity.HasOne(d => d.ContinentCodeNavigation)
                    .WithMany(p => p.Countries)
                    .HasForeignKey(d => d.ContinentCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_countries_continents");
            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.CompanyId, "CompanyId");

                entity.HasIndex(e => e.DestinationId, "DestinationId");

                entity.HasIndex(e => e.OriginId, "OriginId");

                entity.Property(e => e.DestinationId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.OriginId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Company)
                    .WithMany()
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rates_ibfk_1");

                entity.HasOne(d => d.Destination)
                    .WithMany()
                    .HasForeignKey(d => d.DestinationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rates_ibfk_3");

                entity.HasOne(d => d.Origin)
                    .WithMany()
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rates_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
