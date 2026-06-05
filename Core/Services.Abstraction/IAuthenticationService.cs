using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface IAuthenticationService
    {
        // logIn and register
        public Task<UserResultDTO> LogInAsync(LogInDto logInDto);

        public Task<UserResultDTO> RegisterAsync(RegisterDto registerDto);
    }
}
