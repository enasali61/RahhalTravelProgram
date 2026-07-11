using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs.UserDto;

namespace Services.Abstraction
{
    public interface IUserService
    {
        public  Task<UserProfileDto> GetUserProfileAsync();
        public Task<AdminProfileDto> GetDashboardStatsAsync();

    }
}
