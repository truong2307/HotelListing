using HotelListing.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Dto
{
    public class CreateHotelDto
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Hotel name is too long.")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Address is too long.")]
        public string Address { get; set; }
        [Required]
        [Range(1,5)]
        public Double Rating { get; set; }
        [Required]
        public int CountryId { get; set; }
    }

    public class HotelDto : CreateHotelDto
    {
        public int Id { get; set; }
        public CountryDto Country { get; set; }
    }
}
