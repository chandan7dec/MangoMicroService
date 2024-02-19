using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class LoginRequestDto
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
