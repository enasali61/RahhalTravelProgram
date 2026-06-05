using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record RegisterDto
    {
        [Required(ErrorMessage = "Display Name is required")]
        public string DisplayName { get; init; }

        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; init; }

        [Required(ErrorMessage = "Email Name is required")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; }
        public string? PhoneNumber { get; set; }
    }
}
