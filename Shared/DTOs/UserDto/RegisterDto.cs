using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.UserDto
{
    public record RegisterDto
    {
     
        [Required(ErrorMessage = "User Name is required")]
        public string FullName { get; init; }

        [Required(ErrorMessage = "Email Name is required")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; }
        public bool IsEgyptian { get; set; }
        public bool IsStudent { get; set; }
    }
}
