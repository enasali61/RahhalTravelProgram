using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared;
using Shared.DTOs;
using Shared.DTOs.UserDto;

namespace Presentation
{
    [Authorize]
    public class SettingsController(IServiceManager serviceManager) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var settings = await serviceManager.NotificationService.GetSettingsAsync();
            return Ok(new ApiResponse<NotificationSettingsDto>
            {
                Status = "success",
                Data = settings
            });
        }

        // PUT: api/notifications
        [HttpPut]
        public async Task<IActionResult> UpdateNotificationSettings([FromBody] UpdateNotificationSettingsDto dto)
        {
            var settings = await serviceManager.NotificationService.UpdateSettingsAsync( dto);
            return Ok(new ApiResponse<NotificationSettingsDto>
            {
                Status = "success",
                Message = "Notification settings updated successfully",
                Data = settings
            });
        }
        [HttpPut("language")]
        public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageDto dto)
        {

            var result = await serviceManager.NotificationService.UpdateLanguageAsync(dto.Language);
            return Ok(new ApiResponse<UpdateLanguageDto>
            {
                Status = "success",
                Data = result
            });
        }

        // PUT: api/settings/location-sharing
        [HttpPut("location-sharing")]
        public async Task<IActionResult> ToggleLocationSharing([FromBody] ToggleLocationSharingDto dto)
        {

            var result = await serviceManager.NotificationService.ToggleLocationSharingAsync( dto.Enabled);
            return Ok(new ApiResponse<ToggleLocationSharingDto>
            {
                Status = "success",
                Data = result
            });
        }

        // DELETE: api/settings/account
        [HttpDelete("account")]
        public async Task<IActionResult> DeleteAccount()
        {

            var result = await serviceManager.NotificationService.DeleteAccountAsync();
            return Ok(new ApiResponse<bool>
            {
                Status = "success",
                Message = "Account deleted successfully",
                Data = result
            });
        }
    }

}
