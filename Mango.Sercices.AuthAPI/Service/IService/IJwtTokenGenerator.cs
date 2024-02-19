using Mango.Sercices.AuthAPI.Models;

namespace Mango.Sercices.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
