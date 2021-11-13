using AutoMapper;
using HotelListing.Core.Dtos;
using HotelListing.Data;

namespace HotelListing.Core.Configurations
{
    public class MapperInitilizer : Profile
    {
        public MapperInitilizer()
        {
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();
            CreateMap<ApiUser, UserDto>().ReverseMap();
        }
    }
}
