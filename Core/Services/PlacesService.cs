using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    internal class PlacesService(IUnitOfWork unitOfWork, IMapper mapper)
        : IPlacesService
    {

        public async Task<IEnumerable<PlacesResultDTO>> GetAllPlacesAsync()
        {
            // retrive all places
            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();
            // mapping to places dto => imapper
            var placesResult = mapper.Map<IEnumerable<PlacesResultDTO>>(places);
            // return places
            return placesResult;
        }
        public async Task<PlacesResultDTO> GetPlaceByIdAsync(int id)
        {
            // retrive all places
            var places = await unitOfWork.GetRepository<Places>().GetByIdAsync(id);
            //// mapping to places dto => imapper
            //var placesResult = mapper.Map<PlacesResultDTO>(places);
            //// return places
            //return placesResult;
            return places is null ? throw new PlaceNotFoundException(id) : mapper.Map<PlacesResultDTO>(places);

        }


        public async Task<IEnumerable<PlacesResultDTO>> GetPlacesByLocationAsync(string location)
        {
            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();
            var filteredPlaces = places
                .Where(p => p.Location != null &&
                           p.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return mapper.Map<IEnumerable<PlacesResultDTO>>(filteredPlaces);
        }

       
        public async Task<IEnumerable<PlacesResultDTO>> GetTopRatedPlacesAsync(int count = 10)
        {
            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();

            var topPlaces = places
                .Where(p => p.Rating.HasValue)
                .OrderByDescending(p => p.Rating)
                .ThenBy(p => p.Id)
                .Take(count)
                .ToList();

            return mapper.Map<IEnumerable<PlacesResultDTO>>(topPlaces);
        }

        public async Task<IEnumerable<PlacesResultDTO>> SearchPlacesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllPlacesAsync();

            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();

            var searchResults = places
                .Where(p =>
                    (!string.IsNullOrEmpty(p.NameAr) &&
                     p.NameAr.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                     !string.IsNullOrEmpty(p.NameEn) &&
                     p.NameEn.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(p.Description) &&
                     p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Location) &&
                     p.Location.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
            return mapper.Map<IEnumerable<PlacesResultDTO>>(searchResults);
        }

        public async Task<IEnumerable<PlacesResultDTO>> GetPlacesByCategoryAsync(int categoryId)
        {
            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();

            // filter by category id
            var filteredPlaces = places
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            // mapping to places dto => imapper
            var placesResult = mapper.Map<IEnumerable<PlacesResultDTO>>(filteredPlaces);

            // return places
            return placesResult;
        }
    }
}
