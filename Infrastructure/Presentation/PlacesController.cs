using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;
using Shared.ErrorModels;

namespace Presentation
{

    [Authorize]
    public class PlacesController(IServiceManager serviceManager)
        : BaseApiController
    {
        #region Get All Places 
        [HttpGet] // get : baseurl/api/Places
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetAllPlaces()
        {
            var places = await serviceManager.PlacesService.GetAllPlacesAsync();
            return Ok(places);
        }

        #endregion

        #region Get Place by id 
       
        [ProducesResponseType(typeof(PlacesResultDTO), (int)HttpStatusCode.OK)]
        [HttpGet("{id}")] // get : baseurl/api/Places
        public async Task<ActionResult<PlacesResultDTO>> GetPlaceById(int id)
        {
            var place = await serviceManager.PlacesService.GetPlaceByIdAsync(id);
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

        #region Get Places by Location
        [HttpGet("Location/{location}")]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetPlacesByLocation(string location)
        {
            var places = await serviceManager.PlacesService.GetPlacesByLocationAsync(location);
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
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetTopRatedPlaces([FromQuery] int count = 10)
        {
            var places = await serviceManager.PlacesService.GetTopRatedPlacesAsync(count);
            return Ok(places);
        }
        #endregion

       //// #region Get Places Count by Category
       // [HttpGet("Stats/Categories")]
       // public async Task<ActionResult<Dictionary<string, int>>> GetPlacesCountByCategory()
       // {
       //     var stats = await serviceManager.PlacesService.GetPlacesCountByCategoryAsync();
       //     return Ok(stats);
       // }
       // #endregion

    
    }



}
