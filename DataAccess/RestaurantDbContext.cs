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
        }
    }
}
