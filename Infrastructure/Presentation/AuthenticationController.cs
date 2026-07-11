using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared;
using Shared.DTOs;
using Shared.DTOs.UserDto;

namespace Presentation
{
    public class AuthenticationController(IServiceManager serviceManager) : BaseApiController
    {
        // login and register
        [HttpPost("LogIn")] // post: baseUrl/Authentication/LogIn
        public async Task<ActionResult<UserResultDTO>> LogIn(LogInDto logInDto)
        => Ok(await serviceManager.AuthenticationService.LogInAsync(logInDto));

        [HttpPost("Register")] // post: baseUrl/Authentication/Register
        public async Task<ActionResult<UserResultDTO>> Register(RegisterDto registerDto)
        => Ok(await serviceManager.AuthenticationService.RegisterAsync(registerDto));

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await serviceManager.AuthenticationService.ForgotPasswordAsync(dto);
            return Ok(new { message = "If the email exists, a reset link will be sent." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.GetUserId();
                await serviceManager.AuthenticationService.LogoutAsync(userId);

                return Ok(new ApiResponse<string>
                {
                    Status = "success",
                    Message = "Logged out successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Status = "error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Status = "error",
                    Message = $"Logout failed: {ex.Message}"
                });
            }
        }
        // POST: api/auth/reset-password
        [HttpGet("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPasswordPage([FromQuery] string token, [FromQuery] string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("Invalid reset link.");

            var html = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <title>Reset Password - Rahhal</title>
        <style>
            body {{ font-family: Arial, sans-serif; max-width: 400px; margin: 80px auto; padding: 20px; }}
            input {{ width: 100%; padding: 10px; margin: 10px 0; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; }}
            button {{ width: 100%; padding: 12px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
            .message {{ padding: 10px; border-radius: 5px; margin-top: 15px; display: none; }}
            .success {{ background: #d4edda; color: #155724; }}
            .error {{ background: #f8d7da; color: #721c24; }}
        </style>
    </head>
    <body>
        <h2>Reset Your Password</h2>
        <p>Enter your new password below.</p>
        <input type='password' id='newPassword' placeholder='New Password' />
        <input type='password' id='confirmPassword' placeholder='Confirm Password' />
        <button onclick='resetPassword()'>Reset Password</button>
        <div id='message' class='message'></div>

        <script>
            async function resetPassword() {{
                const newPassword = document.getElementById('newPassword').value;
                const confirmPassword = document.getElementById('confirmPassword').value;
                const msg = document.getElementById('message');

                if (!newPassword || !confirmPassword) {{
                    showMessage('Please fill in both fields.', false);
                    return;
                }}
                if (newPassword !== confirmPassword) {{
                    showMessage('Passwords do not match.', false);
                    return;
                }}

                try {{
                    const response = await fetch('/api/Authentication/reset-password', {{
                        method: 'POST',
                        headers: {{ 'Content-Type': 'application/json' }},
                        body: JSON.stringify({{
                            email: '{email}',
                            token: '{token}',
                            newPassword: newPassword
                        }})
                    }});

                    const data = await response.json();

                    if (response.ok) {{
                        showMessage('Password reset successfully! You can now log in with your new password.', true);
                    }} else {{
                        showMessage(data.message || 'Reset failed. The link may have expired.', false);
                    }}
                }} catch (err) {{
                    showMessage('Something went wrong. Please try again.', false);
                }}
            }}

            function showMessage(text, success) {{
                const msg = document.getElementById('message');
                msg.textContent = text;
                msg.className = 'message ' + (success ? 'success' : 'error');
                msg.style.display = 'block';
            }}
        </script>
    </body>
    </html>";

            return Content(html, "text/html");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            return Ok("Password reset successfully.");
        }


    }
}
