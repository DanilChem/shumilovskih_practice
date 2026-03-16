using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Models;

namespace shumilovskih_library.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleHistory> SalesHistory { get; set; }

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Database=shumilovskih_pp;Username=app;Password=123456789"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("app");

            modelBuilder.Entity<Partner>()
                .HasOne(p => p.PartnerType)
                .WithMany(pt => pt.Partners)
                .HasForeignKey(p => p.PartnerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleHistory>()
                .HasOne(sh => sh.Partner)
                .WithMany(p => p.SalesHistory)
                .HasForeignKey(sh => sh.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleHistory>()
                .HasOne(sh => sh.Product)
                .WithMany(p => p.SalesHistory)
                .HasForeignKey(sh => sh.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partner>().ToTable("partners");
            modelBuilder.Entity<PartnerType>().ToTable("partner_types");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<SaleHistory>().ToTable("sales_history");
        }
    }
}