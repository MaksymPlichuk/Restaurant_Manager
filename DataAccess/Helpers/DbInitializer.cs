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
            //modelBuilder.Entity<Category>().HasData(
            //    new Category { Id = 1, Name = "test", Products = }
            //    );
        }
        public static void SeedBooking(this ModelBuilder modelBuilder)
        {
            


        }
        public static void SeedOrder(this ModelBuilder modelBuilder)
        {
            


        }
        public static void SeedProduct(this ModelBuilder modelBuilder)
        {
            


        }
        public static void SeedReview(this ModelBuilder modelBuilder)
        {
            


        }
        public static void SeedUser(this ModelBuilder modelBuilder)
        {
            

        }
    }
}
