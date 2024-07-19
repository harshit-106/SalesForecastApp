using Microsoft.EntityFrameworkCore;
using SalesForecastApp.Models;
using System.Collections.Generic;

namespace SalesForecastApp.Data
{
    public class SalesForecastContext : DbContext
    {
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<OrdersReturns> Returns { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=SalesForcasting;User Id=postgres;Password=admin;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>().ToTable("orders");
            modelBuilder.Entity<Products>().ToTable("products");
            modelBuilder.Entity<OrdersReturns>().ToTable("ordersreturns");
            modelBuilder.Entity<Orders>()
                .HasKey(o => o.orderid);

            modelBuilder.Entity<Products>()
                .HasNoKey(); 

            modelBuilder.Entity<OrdersReturns>()
                .HasKey(r => r.orderid);
        }

    }

}
