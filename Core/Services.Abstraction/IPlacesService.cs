using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.DTOs;
using Shared.DTOs.PlacesDto;

namespace Services.Abstraction
{
    public interface IPlacesService
    {
        
        // basics
        Task<PlacesResultDTO> GetPlaceByIdAsync(int id, string? requestedLanguage = null);
        Task<PaginatedResult<PlacesResultDTO>> GetAllPlacesAsync(PlacesSpecParams placesSpecParams, string? requestedLanguage = null);

        // for search
        Task<PlacesResultDTO> GetPlaceByNameEnAsync(string nameEn, string? requestedLanguage = null);
        Task<IEnumerable<PlacesResultDTO>> GetPlacesByCategoryAsync(int categoryId);
        Task<IEnumerable<PlacesResultDTO>> SearchPlacesAsync(string keyword, string? requestedLanguage = "en");

        // counting
        Task<IEnumerable<PlacesResultDTO>> GetTopRatedPlacesAsync(int count, string? language);

        //// management
        Task<PlacesResultDTO> CreatePlaceAsync(CreatePlaceDto createDto);
        Task<PlacesResultDTO> UpdatePlaceAsync(int id, UpdatePlaceDto updateDto);
        Task<bool> DeletePlaceAsync(int id);
        Task<PaginatedResult<AdminPlaceDto>> GetAllPlacesForAdminAsync(PlacesSpecParams placesSpecParams, string? requestedLanguage = "en");
        // for users
        Task<bool> TogglePlaceFavoriteAsync(int placeId, int userId);  // user chooses place as his favourite 
        Task<IEnumerable<PlacesResultDTO>> GetUserFavoritePlacesAsync(string? language = null); // user sees his fav places all in one place
        Task<string> UploadPlaceImageAsync(int placeId, Stream fileStream, string fileName, string contentType);
    }
}
