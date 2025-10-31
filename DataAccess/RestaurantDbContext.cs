using DataAccess.Entities;
using DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class RestaurantDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"workstation id=RestaurantDatabase.mssql.somee.com;
                                        packet size=4096;user id=OlegVinnuk322_SQLLogin_1;
                                        pwd=gnwrtyu3qb;
                                        data source=RestaurantDatabase.mssql.somee.com;
                                        persist security info=False;
                                        initial catalog=RestaurantDatabase;
                                        TrustServerCertificate=True
            ");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //зв'язки
            //modelBuilder.Entity<Booking>


            //modelBuilder.SeedCategories();
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
}
    }
}
