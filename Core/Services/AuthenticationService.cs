using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using Services.Abstraction;
using Shared;
using Shared.DTOs;
using Shared.DTOs.UserDto;
namespace Services
{
    public class AuthenticationService(
        UserManager<Users> _userManager,
       IOptions<JwtOptions> options,
      IConfiguration configuration,
      ILogger _logger
       ) : IAuthenticationService
    {

        private async Task<string> CreateTokenAsync(Users users)
        {
            var jwtOptions = options.Value;
            // create claims
            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.Name,users.UserName),
                new Claim(ClaimTypes.Email,users.Email),
                new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()),
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
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                signingCredentials: cred
                );
            // object member method => create object from jwtsecurity
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ApiResponse<UserResultDTO>> LogInAsync(LogInDto logInDto)
        {
            // check on email and pass
            var user = await _userManager.FindByEmailAsync(logInDto.Email);
            if (user == null)
                throw new UnAuthorizedException($"email {logInDto.Email} does not exist");
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, logInDto.Password);
            if (!isPasswordValid)
                throw new UnAuthorizedException("Email or password is incorrect");
            // check that pass is correct
            var result = await _userManager.CheckPasswordAsync(user, logInDto.Password);
            if (!result)
                throw new UnAuthorizedException();

            var token = await CreateTokenAsync(user);

            return new ApiResponse<UserResultDTO>
            {
                Status = "success",
                Data = new UserResultDTO
                (
                    user.UserName,
                     user.Email,
                     token
                )
            };                   

        }

        public async Task<ApiResponse<UserResultDTO>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new Users
            {
                Email = registerDto.Email,
                UserName = registerDto.FullName,
                IsEgyptian = registerDto.IsEgyptian,
                IsStudent = registerDto.IsStudent
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}"); // ✅ Now shows actual errors
            }

            var token = await CreateTokenAsync(user);
            return new ApiResponse<UserResultDTO>
            {
                Status = "success",
                Data = new UserResultDTO
               (
                   user.UserName,
                    user.Email,
                    token
               )
            };
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                //  For security, don't reveal if email exists or not.
                // Always return success to prevent email enumeration.
                return;
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(dto.Email);

            // Build reset link (your Flutter app will handle this URL)

            var resetLink = $"https://implant-liberty-transfer.ngrok-free.dev/api/Authentication/reset-password?token={encodedToken}&email={encodedEmail}";
            await SendPasswordResetEmailAsync(dto.Email, resetLink);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid email or token.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password reset failed: {errors}");
            }
        }

        private async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var apiKey = configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("sosoyeol588@gmail.com", "Rahhal Support");
            var to = new EmailAddress(email);
            var subject = "Reset Your Password";
            var plainTextContent = $"Click the link to reset your password: {resetLink}";
            var htmlContent = $@"
     <h2>Reset Your Password</h2>
     <p>Click the button below to reset your password:</p>
     <a href='{resetLink}' style='display:inline-block;padding:10px 20px;background:#007bff;
     color:white;text-decoration:none;border-radius:5px;'>Reset Password</a>
     <p>If you didn't request this, ignore this email.</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var replyTo = new EmailAddress("sosoyeol588@gmail.com", "Rahhal Support");
            msg.ReplyTo = replyTo;
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"📧 SendGrid status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"❌ SendGrid error body: {body}");
            }
            else
            {
                Console.WriteLine("✅ Email accepted by SendGrid.");
            }
        }

        public async Task LogoutAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Log the logout event
            _logger.LogInformation($"User {user.Email} logged out at {DateTime.UtcNow}");
        }
    }
}
