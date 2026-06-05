using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;

namespace Presentation
{
    
    [Authorize]
    public class TripsController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips()
        {
            var trips = await _serviceManager.TripService.GetAllTripsAsync();
            return Ok(trips);
        }

        // GET: api/trips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetTripById(int id)
        {
            var trip = await _serviceManager.TripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound();
            return Ok(trip);
        }

        // POST: api/trips?userId=1
        //[HttpPost]
        //public async Task<ActionResult<TripDto>> CreateTrip([FromQuery] int userId, [FromBody] CreateTripDto createDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var trip = await _serviceManager.TripService.CreateTripAsync(userId, createDto);
        //    return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, trip);
        //}

        // PUT: api/trips/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTrip(int id, [FromBody] UpdateTripDto updateDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        await _serviceManager.TripService.UpdateTripAsync(id, updateDto);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        // DELETE: api/trips/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTrip(int id)
        //{
        //    try
        //    {
        //        await _serviceManager.TripService.DeleteTripAsync(id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        // GET: api/trips/5/places
        [HttpGet("Places/{tripId}")]
        public async Task<ActionResult<IEnumerable<TripPlaceDto>>> GetTripPlaces(int tripId)
        {
            var places = await _serviceManager.TripService.GetTripPlacesAsync(tripId);
            return Ok(places);
        }

        //// POST: api/trips/5/places
        //[HttpPost("{id}/places")]
        //public async Task<ActionResult<TripPlaceDto>> AddPlaceToTrip(int id, [FromBody] AddPlaceToTripDto addDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        var tripPlace = await _serviceManager.TripService.AddPlaceToTripAsync(id, addDto);
        //        return CreatedAtAction(nameof(GetTripPlaces), new { id }, tripPlace);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        //// PUT: api/trips/places/5
        //[HttpPut("places/{tripPlaceId}")]
        //public async Task<IActionResult> UpdateTripPlace(int tripPlaceId, [FromBody] UpdateTripPlaceDto updateDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        await _serviceManager.TripService.UpdateTripPlaceAsync(tripPlaceId, updateDto);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        //// DELETE: api/trips/places/5
        //[HttpDelete("places/{tripPlaceId}")]
        //public async Task<IActionResult> RemovePlaceFromTrip(int tripPlaceId)
        //{
        //    try
        //    {
        //        await _serviceManager.TripService.RemovePlaceFromTripAsync(tripPlaceId);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

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
    }
}
   
