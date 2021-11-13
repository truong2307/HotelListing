using HotelListing.Core.Dtos;
using System.Threading.Tasks;

namespace HotelListing.Core.Servives
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDto userRequest);
        Task<string> CreateToken();
    }
}
