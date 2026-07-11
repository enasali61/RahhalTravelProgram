using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.SubEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared;
using Shared.DTOs;
using Shared.DTOs.TripDto;

namespace Presentation
{
    public class AiRecognitionController(IServiceManager serviceManager ) : BaseApiController
    {
        [HttpPost("recognize-image")]
        public async Task<IActionResult> RecognizeImage(IFormFile image, [FromQuery] string? language = "en")
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "No image uploaded." });

            using var stream = image.OpenReadStream();
            var result = await serviceManager.AiService.RecognizeImageAsync(stream, image.FileName);           

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return StatusCode(502, new { message = result.ErrorMessage });

            var place = await serviceManager.PlacesService.GetPlaceByNameEnAsync(result.Landmark!, language);
           
            if (place == null)
                return NotFound(new
                {
                    message = $"Place '{result.Landmark}' not found in our database.",
                    recognizedName = result.Landmark
                });
            return Ok(new
            {
                recognizedLandmark = result.Landmark,
                placeDetails = place,
                confidence = Math.Round(result.Confidence * 100, 2)
            });
        }

        [HttpPost("generate-itinerary")]
        public async Task<ActionResult<ApiResponse<TripDto>>> GenerateItinerary([FromBody] ItineraryRequestDto request)
        {
            try
            {
                var result = await serviceManager.AiService.GenerateItineraryAsync(request,request.Language);
                return Ok(new ApiResponse<TripDto>
                {
                    Status = "success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                // ✅ Return the actual error message
                return StatusCode(500, new ApiResponse<ItineraryResponseDto>
                {
                    Status = "error",
                    Message = ex.Message,  //  This will show the real error
                    Data = null
                });
            }
        }

        [HttpPost("chat")]
        public async Task<ActionResult<ApiResponse<string>>> Chat([FromBody] ChatRequestDto request)
        {
            try
            {
                var response = await serviceManager.AiService.SendChatMessageAsync(request, request.Message);
                return Ok(new ApiResponse<string>
                {
                    Status = "success",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Status = "error",
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    } }
