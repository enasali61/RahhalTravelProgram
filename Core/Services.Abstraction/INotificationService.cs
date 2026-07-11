using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs.UserDto;

namespace Services.Abstraction
{
    public interface INotificationService
    {
        Task<NotificationSettingsDto> GetSettingsAsync();
        Task<NotificationSettingsDto> UpdateSettingsAsync( UpdateNotificationSettingsDto dto);
        Task<UpdateLanguageDto> UpdateLanguageAsync( string language);
        Task<ToggleLocationSharingDto> ToggleLocationSharingAsync( bool enabled);
        Task<bool> DeleteAccountAsync();
    }
}
