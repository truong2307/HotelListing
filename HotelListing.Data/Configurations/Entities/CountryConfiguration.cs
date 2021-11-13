﻿using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data.Configurations.Entities
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {
                    Id = 1,
                    Name = "Việt Nam",
                    ShortName = "VN"
                }, new Country
                {
                    Id = 2,
                    Name = "American",
                    ShortName = "USA"
                }, new Country
                {
                    Id = 3,
                    Name = "ThaiLand",
                    ShortName = "TL"
                });
        }
    }
}
