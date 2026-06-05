using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;

namespace Presentation
{
    public class AuthenticationController(IServiceManager serviceManager) : BaseApiController
    {
        // login and register
        [HttpPost("LogIn")] // post: baseUrl/Authentication/LogIn
        public async Task<ActionResult<UserResultDTO>> LogIn(LogInDto logInDto)
        => Ok( await serviceManager.AuthenticationService.LogInAsync(logInDto));

        [HttpPost("Register")] // post: baseUrl/Authentication/Register
        public async Task<ActionResult<UserResultDTO>> Register(RegisterDto registerDto)
        => Ok(await serviceManager.AuthenticationService.RegisterAsync(registerDto));

    }
}
