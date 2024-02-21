using Mango.Sercices.AuthAPI.Models;
using Mango.Sercices.AuthAPI.Models.Dto;
using Mango.Sercices.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Data.SqlClient;

namespace Mango.Sercices.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService( AppDbContext db, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
           var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if(user != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();  

                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
           
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower()); 
            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if(user == null ||  isValid == false) {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found, Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user,roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            }; 
            return loginResponseDto;
        }

        public async Task<string> Register(RegisterationRequestDto registerationRequestDto)
        {

            ApplicationUser user = new()
            {
                UserName = registerationRequestDto.Email,
                Email = registerationRequestDto.Email,
                NormalizedEmail = registerationRequestDto.Email.ToUpper(),
                Name = registerationRequestDto.Name,
                PhoneNumber = registerationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDto.Password);
                if(result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registerationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch(Exception ex)
            {

            }
            return "Error Encounter";
        }
    }
}
