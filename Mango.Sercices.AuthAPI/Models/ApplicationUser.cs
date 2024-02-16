using Microsoft.AspNetCore.Identity;

namespace Mango.Sercices.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
