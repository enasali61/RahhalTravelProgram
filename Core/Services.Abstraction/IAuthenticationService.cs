using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.DTOs.UserDto;

namespace Services.Abstraction
{
    public interface IAuthenticationService
    {
        // logIn and register
        public Task<ApiResponse<UserResultDTO>> LogInAsync(LogInDto logInDto);
        public Task<ApiResponse<UserResultDTO>> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync(int userId);
        public Task ForgotPasswordAsync(ForgotPasswordDto dto);
        public Task ResetPasswordAsync(ResetPasswordDto dto);

    }
}
