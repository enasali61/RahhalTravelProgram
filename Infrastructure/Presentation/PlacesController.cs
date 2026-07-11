using System.Net;
using Domain.Entities.TripAndPlaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared;
using Shared.DTOs;
using Shared.DTOs.PlacesDto;
using Shared.ErrorModels;

namespace Presentation
{

    [Authorize]
    public class PlacesController(IServiceManager serviceManager) 
        : BaseApiController
    {
        #region Get All Places 
        [HttpGet] // get : baseurl/api/Places
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetAllPlaces([FromQuery]PlacesSpecParams placesSpecParams, [FromQuery] string language = "en")
        {
            var places = await serviceManager.PlacesService.GetAllPlacesAsync(placesSpecParams, language);
            return Ok(places);
        }

        #endregion

        #region Get Place by id 
        [ProducesResponseType(typeof(PlacesResultDTO), (int)HttpStatusCode.OK)]
        [HttpGet("{id}")] // get : baseurl/api/Places
        public async Task<ActionResult<PlacesResultDTO>> GetPlaceById(int id, [FromQuery] string? language = "en")
        {
            var place = await serviceManager.PlacesService.GetPlaceByIdAsync(id, language);
            return Ok(place);
        }
        #endregion

        #region Get Places by Category
        // GET: api/categories/Places/5
        [HttpGet("Category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetPlacesByCategory(int categoryId)
        {
            var places = await serviceManager.PlacesService.GetPlacesByCategoryAsync(categoryId);
            return Ok(places);
        }

        #endregion

        #region Search Places
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> SearchPlaces([FromQuery] string keyword)
        {
            var places = await serviceManager.PlacesService.SearchPlacesAsync(keyword);
            return Ok(places);
        }
        #endregion

        #region Get Top Rated Places
        [HttpGet("top-rated")]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetTopRatedPlaces([FromQuery] int count = 4, [FromQuery] string? language = "en")
        {
            var places = await serviceManager.PlacesService.GetTopRatedPlacesAsync(count,language);
            return Ok(places);
        }
        #endregion

        #region create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PlacesResultDTO>> CreatePlace(CreatePlaceDto createDto)
        {
            
            var place = await serviceManager.PlacesService.CreatePlaceAsync(createDto);
            return CreatedAtAction(nameof(GetPlaceById), new { id = place.Id }, place);
        }
        #endregion

        #region update
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(int id, UpdatePlaceDto updateDto)
        {
            try
            {                
                var updated = await serviceManager.PlacesService.UpdatePlaceAsync(id, updateDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                throw new PlaceNotFoundException(id);
            }
        }
        #endregion

        #region delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
          
            var deleted = await serviceManager.PlacesService.DeletePlaceAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        #endregion

        #region ToggleFavorite
        [HttpPost("{placeId}/favorite")]
        public async Task<IActionResult> ToggleFavorite(int placeId)
        {
            var userId = User.GetUserId();
            var isFavorited = await serviceManager.PlacesService.TogglePlaceFavoriteAsync(placeId, userId);
            return Ok(new { isFavorited });
        }
        #endregion

        #region GetMyFavorites
        [HttpGet("favorites")]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetMyFavorites([FromQuery] string? language = "en")
        {
            var userId = User.GetUserId();
            var favorites = await serviceManager.PlacesService.GetUserFavoritePlacesAsync(language);
            return Ok(favorites);
        }
        #endregion
       

        #region UploadPlaceImage
        [Authorize(Roles = "Admin")]
        [HttpPost("{placeId}/images")]
        public async Task<IActionResult> UploadPlaceImage(int placeId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No image file provided.");

            try
            {
                // Call the service method
                var imageUrl = await serviceManager.PlacesService.UploadPlaceImageAsync(
                    placeId,
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType
                );

                return Ok(new { imageUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to upload image.", error = ex.Message });
            }

        }
        #endregion

        [HttpGet("places")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<PaginatedResult<AdminPlaceDto>>> GetAllPlacesForAdmin([FromQuery] PlacesSpecParams placesSpecParams, string? requestedLanguage = "en")
        {
            var places = await serviceManager.PlacesService.GetAllPlacesForAdminAsync(placesSpecParams,requestedLanguage);
            return Ok(places);
        }

    }



}
