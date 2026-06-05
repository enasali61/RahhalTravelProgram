using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface IPlacesService
    {
        
        // basics
        Task<PlacesResultDTO> GetPlaceByIdAsync(int id);
        Task<IEnumerable<PlacesResultDTO>> GetAllPlacesAsync();

        // for search
        Task<IEnumerable<PlacesResultDTO>> GetPlacesByCategoryAsync(int categoryId);
        Task<IEnumerable<PlacesResultDTO>> SearchPlacesAsync(string keyword);
        Task<IEnumerable<PlacesResultDTO>> GetPlacesByLocationAsync(string location);

        // counting
        //Task<IEnumerable<string>> GetAllCategoriesAsync();
      //  Task<Dictionary<string, int>> GetPlacesCountByCategoryAsync();
        Task<IEnumerable<PlacesResultDTO>> GetTopRatedPlacesAsync(int count = 10);

        //// management
        //Task<PlacesResultDTO> CreatePlaceAsync(CreatePlacetDTO createDto);
        //Task<PlacesResultDTO> UpdatePlaceAsync(int id, UpdatePlaceDto updateDto);
        //Task<bool> DeletePlaceAsync(int id);
       
        // for users
        //Task<bool> TogglePlaceFavoriteAsync(int placeId, int userId);  // user chooses place as his favourite 
        //Task<IEnumerable<PlacesResultDTO>> GetUserFavoritePlacesAsync(int userId); // user sees his fav places all in one place
    }
}
