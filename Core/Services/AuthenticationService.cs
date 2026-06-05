using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Abstraction;
using Shared;
using Shared.DTOs;

namespace Services
{
    public class AuthenticationService(UserManager<Users> _userManager,
       IOptions<JwtOptions> options) : IAuthenticationService
    {
        public async Task<UserResultDTO> LogInAsync(LogInDto logInDto)
        {
            // check on email and pass
            var user = await _userManager.FindByEmailAsync(logInDto.Email);
            if (user == null)
                throw new UnAuthorizedException($"email {logInDto.Email} does not exist");
            // check that pass is correct
            var result = await _userManager.CheckPasswordAsync(user, logInDto.Password);
            if (!result)
                throw new UnAuthorizedException();
            // generate token & return response
            return new UserResultDTO(
                user.UserName!, user.Email!, await CreateTokenAsync(user));

        }

        public async Task<UserResultDTO> RegisterAsync(RegisterDto registerDto)
        {
            var user = new Users
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            return new UserResultDTO(
                user.UserName, user.Email, await CreateTokenAsync(user));
        }
    
        private async Task<string> CreateTokenAsync(Users users)
        {
            var jwtOptions = options.Value;
            // create claims
            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.Name,users.UserName),
                new Claim(ClaimTypes.Email,users.Email)

            };
            // add roles to claim if exits
            var roles = await _userManager.GetRolesAsync(users);
            foreach (var role in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            // create key 
            //// 2c6f20c4764f9caa624d7796921263857d5658e89764a93a8cf42d6e61c1d823
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            // create Algorithim
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // create token 
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer, // back end baseURL 
                audience: jwtOptions.Audience,
                claims : authClaims,
                expires:DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                signingCredentials: cred
                );
            // object member method => create object from jwtsecurity
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
    }
}
