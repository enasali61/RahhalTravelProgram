using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs.UserDto;

namespace Services
{
    public class NotificationService(IUnitOfWork _unitOfWork,UserManager<Users> _userManager,IHttpContextAccessor _httpContextAccessor) : 
        BaseService(_userManager, _httpContextAccessor),INotificationService
    {
        public async Task<bool> DeleteAccountAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            
            var userId = user.Id;

            // 1. Delete user's trips
            var trips = await _unitOfWork.Set<Trip>().Where(t => t.UserId == userId).ToListAsync();
            _unitOfWork.Set<Trip>().RemoveRange(trips);

            // 2. Delete user's wishlist
            var userPlaces = await _unitOfWork.Set<UserPlaces>().Where(up => up.UserId == userId).ToListAsync();
            _unitOfWork.Set<UserPlaces>().RemoveRange(userPlaces);

            // 3. Delete user's subscriptions
            var subscriptions = await _unitOfWork.Set<UserSubscription>().Where(us => us.UserId == userId).ToListAsync();
            _unitOfWork.Set<UserSubscription>().RemoveRange(subscriptions);

            // 4. Delete user's notification settings
            var notifications = await _unitOfWork.Set<UserNotificationSettings>().Where(n => n.UserId == userId).ToListAsync();
            _unitOfWork.Set<UserNotificationSettings>().RemoveRange(notifications);

            await _unitOfWork.SaveChangesAsync();

            // 5. Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete account: {errors}");
            }

            return true;
        }
    

public async Task<NotificationSettingsDto> GetSettingsAsync()
        {
            var user = await GetCurrentUserAsync();
            var userId = user.Id;

            var settings = await _unitOfWork.Set<UserNotificationSettings>()
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (settings == null)
            {
                // Create default settings if not exist
                settings = new UserNotificationSettings { UserId = userId };
                await _unitOfWork.Set<UserNotificationSettings>().AddAsync(settings);
                await _unitOfWork.SaveChangesAsync();
            }

            return new NotificationSettingsDto
            {
                Id = settings.Id,
                TripReminders = settings.TripReminders,
                LiveArrivalAlerts = settings.LiveArrivalAlerts,
                ServiceDisruptions = settings.ServiceDisruptions
            };
        }

        public async Task<ToggleLocationSharingDto> ToggleLocationSharingAsync(bool enabled)
        {

            var user = await GetCurrentUserAsync();
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.LiveLocationSharing = enabled;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update location sharing: {errors}");
            }

            return new ToggleLocationSharingDto
            {
                Enabled = enabled,
            };
        }

        public async Task<UpdateLanguageDto> UpdateLanguageAsync(string language)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            
            user.Language = language;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update language: {errors}");
            }

            return new UpdateLanguageDto
            {
                Language = language,
            };
        }

        public async Task<NotificationSettingsDto> UpdateSettingsAsync( UpdateNotificationSettingsDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            var userId = user.Id;
            var settings = await _unitOfWork.Set<UserNotificationSettings>()
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (settings == null)
            {
                settings = new UserNotificationSettings { UserId = userId };
                await _unitOfWork.Set<UserNotificationSettings>().AddAsync(settings);
            }

            if (dto.TripReminders.HasValue)
                settings.TripReminders = dto.TripReminders.Value;

            if (dto.LiveArrivalAlerts.HasValue)
                settings.LiveArrivalAlerts = dto.LiveArrivalAlerts.Value;

            if (dto.ServiceDisruptions.HasValue)
                settings.ServiceDisruptions = dto.ServiceDisruptions.Value;

            _unitOfWork.Set<UserNotificationSettings>().Update(settings);
            await _unitOfWork.SaveChangesAsync();

            return new NotificationSettingsDto
            {
                Id = settings.Id,
                TripReminders = settings.TripReminders,
                LiveArrivalAlerts = settings.LiveArrivalAlerts,
                ServiceDisruptions = settings.ServiceDisruptions
            };
        }
    }
}
