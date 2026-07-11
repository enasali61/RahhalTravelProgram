using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Services.Specifications;
using Shared;
using Shared.DTOs;
using Shared.DTOs.PlacesDto;

namespace Services
{
    internal class PlacesService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _webHostEnvironment,
        ITranslationServices translationService, UserManager<Users> _userManager, IHttpContextAccessor _httpContextAccessor)
        : BaseService(_userManager, _httpContextAccessor), IPlacesService
    {

        private async Task<PlacesResultDTO> BuildPlaceDtoAsync(Places place, string language, Users? user)
        {
            var dto = _mapper.Map<PlacesResultDTO>(place);
            bool needsSave = false;

            if (language == "ar")
            {
                dto.Name = !string.IsNullOrEmpty(place.NameAr) ? place.NameAr : place.NameEn;
                dto.City = !string.IsNullOrEmpty(place.CityAr) ? place.CityAr : place.CityEn;
                dto.Category = !string.IsNullOrEmpty(place.Category?.NameAr) ? place.Category.NameAr : place.Category?.NameEn;

                if (!string.IsNullOrEmpty(place.HistoricalBackGroundAr) && !string.IsNullOrEmpty(place.VisitingTimeAr))
                {
                    dto.HistoricalBackGround = place.HistoricalBackGroundAr;
                    dto.VisitingTime = place.VisitingTimeAr;
                }

                else if (!string.IsNullOrEmpty(place.HistoricalBackGroundEn) && !string.IsNullOrEmpty(place.VisitingTimeEn))
                {
                    var translatedHistoricalBackGround = await translationService.TranslateToArabicAsync(place.HistoricalBackGroundEn);
                    var translatedVisitingTime = await translationService.TranslateToArabicAsync(place.VisitingTimeEn);
                    dto.HistoricalBackGround = translatedHistoricalBackGround;
                    dto.VisitingTime = translatedVisitingTime;
                    place.HistoricalBackGroundAr = translatedHistoricalBackGround;
                    place.VisitingTimeAr = translatedVisitingTime;
                    needsSave = true;
                }
            }
            else
            {
                dto.Name = place.NameEn;
                dto.City = place.CityEn;
                dto.HistoricalBackGround = place.HistoricalBackGroundEn;
                dto.VisitingTime = place.VisitingTimeEn;
                dto.Category = place.Category.NameEn;
            }

            dto.Price = user != null
                ? (user.IsEgyptian ? (user.IsStudent ? place.PriceEgStudent : place.PriceEgAdult)
                                    : (user.IsStudent ? place.PriceForeignStudent : place.PriceForeignAdult))
                : place.PriceForeignAdult;

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return dto;
        }
        public async Task<PlacesResultDTO?> GetPlaceByNameEnAsync(string nameEn, string? requestedLanguage)
        {
            var place = await _unitOfWork.Set<Places>()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.NameEn == nameEn);
            var user = await GetCurrentUserAsync();
            if (place == null) return null;
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");
            //await _unitOfWork.SaveChangesAsync();
            return await BuildPlaceDtoAsync(place, language, user); // reuse your existing logic
        }
        public async Task<List<PlacesResultDTO>> GetPlacesByNamesAsync(List<string> names, string? requestedLanguage)
        {
            var places = await _unitOfWork.Set<Places>()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => names.Contains(p.NameEn))
                .ToListAsync();
            var user = await GetCurrentUserAsync();
            if (places == null) return null;
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");
            return _mapper.Map<List<PlacesResultDTO>>(places);
        }
        public async Task<PaginatedResult<PlacesResultDTO>> GetAllPlacesAsync(PlacesSpecParams placesSpecParams, string? requestedLanguage = "en")
        {
            var places = (await _unitOfWork.GetRepository<Places>().GetAllAsync(new PlacesWithImageSpec(placesSpecParams))).ToList();
            var totalCount = await _unitOfWork.GetRepository<Places>().CountAsync(new PlacesWithImageSpec(placesSpecParams));

            var user = await GetCurrentUserAsync(); // fetched ONCE, not per place
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");

            var dtos = new List<PlacesResultDTO>();
            foreach (var place in places)
            {
                dtos.Add(await BuildPlaceDtoAsync(place, language, user));
            }

            await _unitOfWork.SaveChangesAsync(); // ONE save for the whole page

            return new PaginatedResult<PlacesResultDTO>(
                placesSpecParams.PageIndex,
                dtos.Count,
                totalCount,
                dtos
            );

        }
        public async Task<PlacesResultDTO> GetPlaceByIdAsync(int id, string? requestedLanguage = null)
        {
            // 1. Get place with images
            var place = await _unitOfWork.GetRepository<Places>().GetByIdAsync(new PlacesWithImageSpec(id));
            if (place is null)
                throw new PlaceNotFoundException(id);
            var dto = _mapper.Map<PlacesResultDTO>(place);
            bool needsSave = false;
            var user = await GetCurrentUserAsync();
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");
            await _unitOfWork.SaveChangesAsync();
            return await BuildPlaceDtoAsync(place, language, user);
        }
        
        ///done
        public async Task<IEnumerable<PlacesResultDTO>> GetTopRatedPlacesAsync(int count = 4, string language = "en")
        {
            // 1. Fetch places (without enrichment)
            var places = await _unitOfWork.GetRepository<Places>()
                .GetAllAsync();
            var top = places
                .Where(p => p.Rating.HasValue)
                .OrderByDescending(p => p.Rating)
                .ThenBy(p => p.Id)
                .Take(count)
                .ToList();
            
            // 2. Get user and resolve language (same as trips)
            var user = await GetCurrentUserAsync();
            string resolvedLanguage = (language == "ar" || language == "en")
                ? language
                : (user?.IsEgyptian == true ? "ar" : "en");

            // 3. Map and enrich each place using BuildPlaceDtoAsync
            var placeDtos = new List<PlacesResultDTO>();
            bool needsSave = false;

            foreach (var place in top)
            {
                var dto = await BuildPlaceDtoAsync(place, resolvedLanguage, user);
                placeDtos.Add(dto);

                // If Google API enriched HistoricalBackGround, mark for save
                if (dto.HistoricalBackGround != null && place.HistoricalBackGroundAr == null && resolvedLanguage == "ar")
                    needsSave = true;
            }

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return placeDtos;
        }
        public async Task<IEnumerable<PlacesResultDTO>> SearchPlacesAsync(string keyword, string? requestedLanguage = "en")
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<PlacesResultDTO>();

            // 1. Query the DbContext Set directly to handle the relations and filter on the database side
            var places = await _unitOfWork.Set<Places>()
                .Include(p => p.Category) // Explicitly load Category to prevent nulls
                .Where(p =>
                    (!string.IsNullOrEmpty(p.NameAr) && p.NameAr.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(p.NameEn) && p.NameEn.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(p.Category.NameEn) && p.Category.NameEn.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(p.Category.NameAr) && p.Category.NameAr.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(p.CityEn) && p.CityEn.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(p.CityAr) && p.CityAr.Contains(keyword))
                )
                .ToListAsync();

            // 2. Resolve language dynamically just like you did in GetAllPlacesAsync
            var user = await GetCurrentUserAsync();
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");

            // 3. Loop through the database results and build the DTOs using your factory method
            var dtos = new List<PlacesResultDTO>();
            foreach (var place in places)
            {
                dtos.Add(await BuildPlaceDtoAsync(place, language, user));
            }

            // 4. Persist any state tracked or computed during the DTO creation 
            await _unitOfWork.SaveChangesAsync();

            return dtos;
        }
        public async Task<IEnumerable<PlacesResultDTO>> GetPlacesByCategoryAsync(int categoryId)
        {
            var places = await _unitOfWork.GetRepository<Places>().GetAllAsync();

            // filter by category id
            var filteredPlaces = places
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            // mapping to places dto => imapper
            var placesResult = _mapper.Map<IEnumerable<PlacesResultDTO>>(filteredPlaces);

            // return places
            return placesResult;
        }


        // ------------------------ Management (Admin) ------------------------
        public async Task<PaginatedResult<AdminPlaceDto>> GetAllPlacesForAdminAsync(PlacesSpecParams placesSpecParams, string? requestedLanguage = "en")
        {
            // 1. Fetch the data using your exact Specification implementation
            var places = (await _unitOfWork.GetRepository<Places>().GetAllAsync(new PlacesWithImageSpec(placesSpecParams))).ToList();
            var totalCount = await _unitOfWork.GetRepository<Places>().CountAsync(new PlacesWithImageSpec(placesSpecParams));

            // 2. Fetch admin user context and resolve language fallback logic
            var user = await GetCurrentUserAsync();
            string language = (requestedLanguage == "ar" || requestedLanguage == "en")
                ? requestedLanguage
                : (user?.IsEgyptian == true ? "ar" : "en");

            var dtos = new List<AdminPlaceDto>();

            // 3. Loop and build the custom admin DTOs with translation fallbacks
            foreach (var place in places)
            {
                string name = language == "ar"
                    ? (!string.IsNullOrEmpty(place.NameAr) ? place.NameAr : place.NameEn)
                    : place.NameEn;

                string city = language == "ar"
                    ? (!string.IsNullOrEmpty(place.CityAr) ? place.CityAr : place.CityEn)
                    : place.CityEn;
                var categoryName = language == "ar"
                    ? (!string.IsNullOrEmpty(place.Category?.NameAr) ? place.Category.NameAr : place.Category?.NameEn)
                    : place.Category?.NameEn;
                string imageUrl = place.Images?
                    .FirstOrDefault(img => img.IsMain)?.ImageUrl
                    ?? place.Images?.FirstOrDefault()?.ImageUrl
                    ?? "";

                dtos.Add(new AdminPlaceDto
                {
                    Id = place.Id,
                    Name = name,
                    CategoryName = categoryName, // If you have a Category relation, use place.Category.Name here
                    City = city,
                    ImageUrl = imageUrl,
                    PriceEgAdult = place.PriceEgAdult,
                    PriceForeignAdult = place.PriceForeignAdult,
                    PriceEgStudent = place.PriceEgStudent,
                    PriceForeignStudent = place.PriceForeignStudent
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return new PaginatedResult<AdminPlaceDto>(
                placesSpecParams.PageIndex,
                dtos.Count,
                totalCount,
                dtos
            );
        }
        public async Task<PlacesResultDTO> CreatePlaceAsync(CreatePlaceDto createDto)
        {
            var place = _mapper.Map<Places>(createDto);
            await _unitOfWork.Set<Places>().AddAsync(place);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PlacesResultDTO>(place);
        }

        public async Task<PlacesResultDTO> UpdatePlaceAsync(int id, UpdatePlaceDto updateDto)
        {
            var place = await _unitOfWork.Set<Places>().FindAsync(id);
            if (place == null) throw new KeyNotFoundException($"Place {id} not found");

            _mapper.Map(updateDto, place);
            _unitOfWork.Set<Places>().Update(place);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PlacesResultDTO>(place);
        }

        public async Task<bool> DeletePlaceAsync(int id)
        {
            var place = await _unitOfWork.Set<Places>().FindAsync(id);
            if (place == null) return false;

            // Remove related images first (optional, cascade may handle)
            var images = await _unitOfWork.Set<PlaceImages>().Where(pi => pi.PlaceId == id).ToListAsync();
            _unitOfWork.Set<PlaceImages>().RemoveRange(images);
            _unitOfWork.Set<Places>().Remove(place);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ------------------------ User Favorites (Wishlist) ------------------------

        public async Task<bool> TogglePlaceFavoriteAsync(int placeId, int userId)
        {
            var existing = await _unitOfWork.Set<UserPlaces>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PlaceId == placeId);

            if (existing != null)
            {
                _unitOfWork.Set<UserPlaces>().Remove(existing);
                await _unitOfWork.SaveChangesAsync();
                return false; // removed from favorites
            }
            else
            {
                var userPlace = new UserPlaces { UserId = userId, PlaceId = placeId };
                await _unitOfWork.Set<UserPlaces>().AddAsync(userPlace);
                await _unitOfWork.SaveChangesAsync();
                return true; // added to favorites
            }
        }

        public async Task<IEnumerable<PlacesResultDTO>> GetUserFavoritePlacesAsync(string? language = null)
        {
            var user = await GetCurrentUserAsync(); // favorites require login

            var favoritePlaces = await _unitOfWork.Set<UserPlaces>()
                .Where(up => up.UserId == user.Id)
                .Include(up => up.Place)
                    .ThenInclude(p => p.Images)      // 👈 needed for mainImageUrl
                .Include(up => up.Place)
                    .ThenInclude(p => p.Category)    // 👈 needed for category name
                .Select(up => up.Place)
                .ToListAsync();

            string resolvedLanguage = (language == "ar" || language == "en")
                ? language
                : (user.IsEgyptian ? "ar" : "en");

            var dtos = new List<PlacesResultDTO>();
            bool needsSave = false;

            foreach (var place in favoritePlaces)
            {
                if (place == null) continue;
                var dto = await BuildPlaceDtoAsync(place, resolvedLanguage, user);
                dto.IsFavorite = true; // they're all favorites by definition here
                dtos.Add(dto);

                if (dto.HistoricalBackGround != null && place.HistoricalBackGroundAr == null && resolvedLanguage == "ar")
                    needsSave = true;
            }

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return dtos;
        }
        public async Task<string> UploadPlaceImageAsync(int placeId, Stream fileStream, string fileName, string contentType)
        {
            // 1. Check if place exists
            var place = await _unitOfWork.Set<Places>().FindAsync(placeId);
            if (place == null)
                throw new KeyNotFoundException($"Place with ID {placeId} not found");

            // 2. Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".jfif" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Only {string.Join(", ", allowedExtensions)} are allowed.");

            // 3. Generate unique filename (use placeId + extension)
            var newFileName = $"{placeId}{extension}";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "places");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, newFileName);

            // 4. Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            // 5. Check if this is the first image for this place
            var existingImages = await _unitOfWork.Set<PlaceImages>()
                .Where(pi => pi.PlaceId == placeId)
                .ToListAsync();

            // 6. Save to PlaceImages table
            var placeImage = new PlaceImages
            {
                PlaceId = placeId,
                ImageUrl = $"/images/places/{newFileName}",
                IsMain = !existingImages.Any(),
                DisplayOrder = existingImages.Count
            };

            await _unitOfWork.Set<PlaceImages>().AddAsync(placeImage);
            await _unitOfWork.SaveChangesAsync();

            // 7. Return the image URL
            return placeImage.ImageUrl;
        }
    }
  
    
}
