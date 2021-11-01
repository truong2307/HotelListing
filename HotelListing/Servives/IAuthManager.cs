using HotelListing.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Servives
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDto userRequest);
        Task<string> CreateToken();
    }
}
