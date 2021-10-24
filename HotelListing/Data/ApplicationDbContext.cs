using HotelListing.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData
            (
                new Country
                {
                    Id = 1,
                    Name = "Việt Nam",
                    ShortName = "VN"
                },
                new Country
                {
                    Id = 2,
                    Name = "American",
                    ShortName = "USA"
                },
                new Country
                {
                    Id = 3,
                    Name = "ThaiLand",
                    ShortName = "TL"
                }
            );

            builder.Entity<Hotel>().HasData
            (
                new Hotel
                {
                    Id = 1,
                    Name = "Mường Thanh",
                    Address = "DaNang",
                    CountryId = 1,
                    Rating = 5
                },
                 new Hotel
                 {
                     Id = 2,
                     Name = "South Oceanside",
                     Address = "California",
                     CountryId = 2,
                     Rating = 4.5
                 },
                  new Hotel
                  {
                      Id = 3,
                      Name = "Sri Panwa",
                      Address = "Muang Phuket",
                      CountryId = 3,
                      Rating = 4.8
                  }
            );
        }

    }
}
