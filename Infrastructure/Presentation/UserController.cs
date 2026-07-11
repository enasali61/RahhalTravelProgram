using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;
using Shared.DTOs.UserDto;


namespace Presentation
{
    [Authorize]
    public class UserController(IServiceManager serviceManager) : BaseApiController
    {
      
        [HttpGet("profile")]
        
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var profile = await serviceManager.UserService.GetUserProfileAsync();
            return Ok(profile);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<AdminProfileDto>> GetDashboardStats()
        {
            var stats = await serviceManager.UserService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}
