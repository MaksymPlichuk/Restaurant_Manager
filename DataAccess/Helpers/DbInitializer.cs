using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    internal static class DbInitializer
    {
        public static void SeedCategories(this ModelBuilder modelBuilder)
        {
            //seeder
            modelBuilder.Entity<Category>().HasData(new Category[]
            {
            new Category { Id = 1, Name = "Appetizers"  },
            new Category { Id = 2, Name = "Salads" },
            new Category { Id = 3, Name = "Desserts" },
            new Category { Id = 4, Name = "Soups" },
            new Category { Id = 5, Name = "Beverages" }
            });
        }
        public static void SeedBooking(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>().HasData(new Booking[]
            {
                new Booking
                {
                    Id = 1,
                    UserId = 1,
                    BookingDate = DateTime.Now.AddDays(1).Date.AddHours(19),
                    AmountOfTables = 2
                },
                new Booking
                {
                    Id = 2,
                    UserId = 2,
                    BookingDate = DateTime.Now.AddDays(2).Date.AddHours(20),
                    AmountOfTables = 1
                }

            });


        }
        public static void SeedProduct(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(new Product[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Bruschetta",
                    Price = 6.99,
                    CategoryId = 1,
                    OrderId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Caesar Salad",
                    Price = 8.99,
                    CategoryId = 2,
                    OrderId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Chocolate Lava Cake",
                    Price = 7.99,
                    CategoryId = 3,
                    OrderId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Tomato Soup",
                    Price = 5.99,
                    CategoryId = 4,
                    OrderId = 1
                },
                new Product
                {
                    Id = 5,
                    Name = "Lemonade",
                    Price = 2.99,
                    CategoryId = 5,
                    OrderId = 2
                }

            });


        }
        public static void SeedReview(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>().HasData(new Review[]
                {
                new Review
                {
                    Id = 1,
                    UserId = 1,
                    Rating = 5,
                    Content = "Excellent food and service!"
                },
                new Review
                {
                    Id = 2,
                    UserId = 2,
                    Rating = 4,
                    Content = "Great atmosphere, will come again."
                }
            });


        }
        public static void SeedUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User[]
            {
                new User
                {
                    Id = 1,
                    Login = "john_doe",
                    Password = "password123",
                    Role = UserRole.Customer
                },
                new User
                {
                    Id = 2,
                    Login = "jane_smith",
                    Password = "password12",
                    Role = UserRole.Customer
                },
                new User {
                    Id = 3,
                    Login = "ron_davis",
                    Password = "password1234",
                    Role = UserRole.Manager
                },
                new User
                {
                    Id = 4,
                    Login = "manager_1",
                    Password = "adminpass",
                    Role = UserRole.Manager
                }
            });

        }
        public static void SeedOrder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    OrderDate = DateTime.Now.AddHours(-2),
                    Status = OrderStatus.New
                },
                new Order
                {
                    Id = 2,
                    UserId = 1,
                    OrderDate = DateTime.Now.AddHours(-1),
                    Status = OrderStatus.Preparing
                },
                new Order
                {
                    Id = 3,
                    UserId = 1,
                    OrderDate = DateTime.Now.AddMinutes(-30),
                    Status = OrderStatus.Ready
                }
            );
        }
    }
}
