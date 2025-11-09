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

            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingDate)
                .IsRequired();
            modelBuilder.Entity<Booking>()
                .Property(b => b.AmountOfTables)
                .IsRequired();
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .IsRequired();
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithOne(p => p.Order);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Products);

            modelBuilder.Entity<Review>()
                .Property(r => r.Content)
                .IsRequired();
            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .IsRequired();
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews);
            modelBuilder.Entity<Review>()
                .Property(r => r.ReviewDate)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Login)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired();
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category);
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue(OrderStatus.New)
                .IsRequired();

            modelBuilder.SeedCategories();
            modelBuilder.SeedBooking();
            modelBuilder.SeedOrder();
            modelBuilder.SeedProduct();
            modelBuilder.SeedReview();
            modelBuilder.SeedUser();
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
}
    }

