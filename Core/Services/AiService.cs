using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Abstraction;
using Shared.DTOs;
using Shared.DTOs.PlacesDto;
using Shared.DTOs.TripDto;

namespace Services
{
    public class AiService(HttpClient _httpClient, ILogger<AiService> _logger, IUnitOfWork _unitOfWork, IMapper mapper,
        IHttpContextAccessor _httpContextAccessor,UserManager<Users> userManager) : BaseService(userManager,_httpContextAccessor), IAiService
    {

        #region image recognation
        public async Task<AiRecognitionResult> RecognizeImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                Console.WriteLine($" Sending image to AI: {fileName}");

                // ✅ Use a fresh HttpClient with SSL bypass
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(120);

                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(imageStream);
                var contentType = fileName.ToLower() switch
                {
                    var f when f.EndsWith(".png") => "image/png",
                    var f when f.EndsWith(".webp") => "image/webp",
                    _ => "image/jpeg"
                };

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "image", fileName);

                var aiServiceUrl = "https://empathy-overhear-unrobed.ngrok-free.dev/image/predict";
                Console.WriteLine($"📤 Calling AI: {aiServiceUrl}");

                var response = await client.PostAsync(aiServiceUrl, content);

                Console.WriteLine($"📥 Status Code: {response.StatusCode}");
                Console.WriteLine($"📥 Content-Type: {response.Content.Headers.ContentType}");
                Console.WriteLine($"📥 Content-Length: {response.Content.Headers.ContentLength ?? 0}");

                // ✅ Read the response as a string FIRST
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📥 Raw Response Length: {jsonResponse.Length}");

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    Console.WriteLine("❌ Empty response body");
                    return new AiRecognitionResult { ErrorMessage = "AI service returned empty response." };
                }

                // ✅ Log the first 200 characters to see the structure
                Console.WriteLine($"📥 Raw Response (first 200 chars): {jsonResponse.Substring(0, Math.Min(200, jsonResponse.Length))}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<AiRecognitionResult>(jsonResponse, options);

                if (result == null)
                {
                    Console.WriteLine("❌ Failed to parse JSON");
                    return new AiRecognitionResult { ErrorMessage = "Failed to parse AI response." };
                }

                Console.WriteLine($"✅ Parsed: Landmark='{result.Landmark}', Confidence={result.Confidence}");
                return result;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"❌ Timeout: {ex.Message}");
                return new AiRecognitionResult { ErrorMessage = "The AI service took too long to respond." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return new AiRecognitionResult { ErrorMessage = $"An error occurred: {ex.Message}" };
            }
        }
        #endregion

        #region generate trip
        public async Task<TripDto> GenerateItineraryAsync(ItineraryRequestDto request, string? language = null)
        {
            try
            {
                var aiUrl = "https://empathy-overhear-unrobed.ngrok-free.dev/trip/generate-itinerary";

                var aiRequest = new
                {
                    city = request.City,
                    duration_days = request.DurationDays,
                    category = request.Category,
                    budget = request.Budget,
                    is_egyptian = request.IsEgyptian,
                    is_student = request.IsStudent,
                    travelers_count = request.TravelersCount
                };

                var json = JsonSerializer.Serialize(aiRequest);
                Console.WriteLine($"📤 AI Request: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ Use a longer timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
                var response = await _httpClient.PostAsync(aiUrl, content, cts.Token);

                Console.WriteLine($"📥 Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ AI Error: {error}");
                    throw new Exception($"AI service error: {response.StatusCode} - {error}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📥 Raw Response: {responseJson}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var aiResult = JsonSerializer.Deserialize<AiItineraryResponse>(responseJson, options);

                if (aiResult == null)
                    throw new Exception("AI service returned invalid response");

                // ✅ Get all place names from the AI response
                var placeNames = aiResult.Schedule
                    .SelectMany(kv => kv.Value ?? new List<AiDaySchedule>())
                    .Select(s => s?.PlaceName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();

                // ✅ Fetch full place data from the database
                var places = await _unitOfWork.Set<Places>()
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Where(p => placeNames.Contains(p.NameEn))
                    .ToDictionaryAsync(p => p.NameEn);

                var userId = await GetCurrentUserIdAsync();
                var favoritePlaceIds = await _unitOfWork.Set<UserPlaces>()
                 .Where(up => up.UserId == userId)
                 .Select(up => up.PlaceId)
                 .ToListAsync();

                // ✅ Build TripDto
                var tripPlaces = new List<TripPlaceDto>();
                var dayNumber = 0;

                // ✅ Calculate total cost
                var totalCost = aiResult.Schedule
                    .SelectMany(kv => kv.Value ?? new List<AiDaySchedule>())
                    .Sum(s => s.CostPerPerson * request.TravelersCount);

                var lang = language ?? request.Language ?? "en";
                var isArabic = lang.ToLower() == "ar";

                foreach (var day in aiResult.Schedule)
                {
                    dayNumber++;
                    var order = 0;
                    foreach (var dayPlace in day.Value)
                    {
                        order++;
                        var place = places.GetValueOrDefault(dayPlace.PlaceName);

                        // ✅ Calculate price per person
                        var pricePerPerson = place != null
                            ? request.IsEgyptian
                                ? (request.IsStudent ? place.PriceEgStudent : place.PriceEgAdult)
                                : (request.IsStudent ? place.PriceForeignStudent : place.PriceForeignAdult)
                            : dayPlace.CostPerPerson;

                        var estimatedCost = pricePerPerson * request.TravelersCount;

                        // ✅ Build place DTO
                        var placeDto = new PlacesResultDTO
                        {
                            Id = place?.Id ?? 0,
                            Name = isArabic ? place?.NameAr : place?.NameEn,
                            City = isArabic ? place?.CityAr : place?.CityEn,
                            Price = pricePerPerson,
                            VisitingTime = isArabic ? place?.VisitingTimeAr : place?.VisitingTimeEn,
                            Category = place?.Category?.NameEn ?? place?.Category.NameAr,
                            HistoricalBackGround = place?.HistoricalBackGroundEn ?? place?.HistoricalBackGroundAr,
                            Longitude = place?.Longitude ?? dayPlace.Longitude,
                            Latitude = place?.Latitude ?? dayPlace.Latitude,
                            MainImageUrl = place?.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                                          ?? place?.Images?.FirstOrDefault()?.ImageUrl
                                          ?? "",
                            GalleryImages = place?.Images?.Where(i => !i.IsMain).OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
                                          ?? new List<string>(),
                            IsFavorite = favoritePlaceIds.Contains(place?.Id ?? 0)
                        };

                        tripPlaces.Add(new TripPlaceDto
                        {
                            Place = placeDto,
                            VisitDate = DateTime.Today.AddDays(dayNumber - 1),
                            VisitOrder = order,
                            StartTime = TryParseTime(dayPlace.StartTime),
                            EndTime = TryParseTime(dayPlace.EndTime),
                            EstimatedCost = estimatedCost,
                            Notes = $"Day {dayNumber}",
                            IsCompleted = false
                        });
                    }
                }

                return new TripDto
                {
                    TripName = $"{request.City} Trip",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(request.DurationDays - 1),
                    TotalBudget = totalCost,
                    DurationDays = request.DurationDays,
                    TravelersCount = request.TravelersCount,
                    Status = "Planned",
                    Notes = $"AI-generated: {request.Category} in {request.City} (budget: {request.Budget} EGP)",
                    TripPlaces = tripPlaces
                };
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"❌ Timeout: {ex.Message}");
                throw new Exception("AI service timed out. Please try again.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        private TimeSpan? TryParseTime(string time)
        {
            if (string.IsNullOrEmpty(time))
                return null;

            // Try 12-hour format (e.g., "09:00 AM")
            if (DateTime.TryParseExact(time, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.TimeOfDay;

            // Try 24-hour format (e.g., "09:00")
            if (TimeSpan.TryParse(time, out var ts))
                return ts;

            return null;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }





        #endregion
        public async Task<string> SendChatMessageAsync(ChatRequestDto request, string message)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                    return "User not found. Please log in again.";

                var aiUrl = "https://empathy-overhear-unrobed.ngrok-free.dev/chatbot/chat";

                var aiRequest = new
                {
                    user_id = user.Id.ToString(),
                    message = message,
                    is_egyptian = user.IsEgyptian,   
                    is_student = user.IsStudent      
                };

                var json = JsonSerializer.Serialize(aiRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(aiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ AI Chatbot Error: {response.StatusCode} - {error}");
                    return "Sorry, I couldn't process your request. Please try again later.";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ChatResponseDto>(responseJson, options);

                return result?.Response ?? "I didn't understand that. Could you rephrase?";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chatbot Exception: {ex.Message}");
                return "An error occurred while processing your message.";
            }
        }
    }
}
