using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared;
using Shared.DTOs.TripDto;

namespace Presentation
{

    //[Authorize]
    public class TripsController(IServiceManager _serviceManager) : BaseApiController
    {
        #region Get all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips([FromQuery] TripSpecParams tripSpec)
        {
            var trips = await _serviceManager.TripService.GetAllTripsAsync(tripSpec);
            return Ok(trips);
        }
        #endregion

        #region Get by id
        // GET: api/trips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetTripById(int id,string ?language ="en")
        {
            var trip = await _serviceManager.TripService.GetTripByIdAsync(id,language);
            if (trip == null)
                return NotFound();
            return Ok(trip);
        }
        #endregion

        #region popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetPopularTrips([FromQuery] int count = 4, [FromQuery] string? language = "en")
        {
            var trips = await _serviceManager.TripService.GetPopularTripsAsync(count);
            return Ok(trips);
        }
        #endregion

        #region Get all trip places 
        // GET: api/trips/5/places
        [HttpGet("{id}/places")]
        public async Task<ActionResult<IEnumerable<TripPlaceDto>>> GetTripPlaces(int id)
        {
            var places = await _serviceManager.TripService.GetTripPlacesAsync(id);
            return Ok(places);
        }
        #endregion

        #region Get my trips
        // GET: api/trips/user/me (or use query param? but better separate)
        [HttpGet("MyTrip")]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetMyTrips([FromQuery] string? language = "en")
        {
            var userId = User.GetUserId();
            var trips = await _serviceManager.TripService.GetUserTripsAsync(language);
            return Ok(trips);
        }
        #endregion

        #region create
        // POST: api/trips
        [HttpPost]
        public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var trip = await _serviceManager.TripService.CreateTripAsync(createDto);
            return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, trip);
        }
        #endregion

        #region confirm
        // POST: api/trips/confirm (convert basket to trip)
        [HttpPost("confirm")]
        public async Task<ActionResult<TripDto>> ConfirmBasket()
        {
            var userId = User.GetUserId();
            var trip = await _serviceManager.TripService.ConfirmBasketAndCreateTripAsync(userId);
            return Ok(trip);
        }
        #endregion

        #region add place to trip
        // POST: api/trips/5/places
        [HttpPost("{tripId}/places")]
        public async Task<ActionResult<TripPlaceDto>> AddPlaceToTrip(int tripId, [FromBody] AddPlaceToTripDto addDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.GetUserId();
                var isAdmin = User.IsInRole("Admin");
                var tripPlace = await _serviceManager.TripService.AddPlaceToTripAsync(tripId, addDto, userId, isAdmin);
                return CreatedAtAction(nameof(GetTripPlaces), new { id = tripId }, tripPlace);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region update

        // PUT: api/trips/5/places/10
        [HttpPut("{tripId}/places/{placeId}")]
        public async Task<IActionResult> UpdateTripPlace(int tripId, int placeId, [FromBody] UpdateTripPlaceDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.GetUserId();
                var isAdmin = User.IsInRole("Admin");
                await _serviceManager.TripService.UpdateTripPlaceAsync(tripId, placeId, updateDto, userId, isAdmin);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        #endregion

        #region delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var isAdmin = User.IsInRole("Admin");
                await _serviceManager.TripService.DeleteTripAsync(id, userId, isAdmin);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }     
        #endregion

        #region remove place from trip

        // DELETE: api/trips/5/places/10
        [HttpDelete("{tripId}/places/{placeId}")]
            public async Task<IActionResult> RemovePlaceFromTrip(int tripId, int placeId)
            {
                try
                {
                    var userId = User.GetUserId();
                    var isAdmin = User.IsInRole("Admin");
                    await _serviceManager.TripService.RemovePlaceFromTripAsync(tripId, placeId, userId, isAdmin);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }
            #endregion

        #region calculate trip cost
            // GET: api/trips/5/cost

            [HttpGet("Cost/{id}")]
            public async Task<ActionResult<decimal>> CalculateTripCost(int id)
            {
                try
                {
                    var cost = await _serviceManager.TripService.CalculateTripCostAsync(id);
                    return Ok(new { totalCost = cost });
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
        #endregion

        #region get daily schedule
        // GET: api/trips/5/schedule
        [HttpGet("Schedule/{TripId}")]
            public async Task<IActionResult> GetDailySchedule(int TripId)
            {
                try
                {
                    var schedule = await _serviceManager.TripService.GetTripDailyScheduleAsync(TripId);
                    return Ok(schedule);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
        #endregion

        //[HttpGet("templates")]
        //public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTemplateTrips([FromQuery] string? language = "en")
        //{
        //    var trips = await _serviceManager.TripService.GetAllTemplateTripsAsync(language);
        //    return Ok(trips);
        //}

    }
}


 