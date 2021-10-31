using HotelListing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData
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
