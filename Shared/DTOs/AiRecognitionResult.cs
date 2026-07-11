using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.DTOs.PlacesDto;
using Shared.DTOs.UserDto;

namespace Shared.DTOs
{
    public class AiRecognitionResult
    {
        [JsonPropertyName("class_id")]
        public int? ClassId { get; set; }

        [JsonPropertyName("landmark")]
        public string? Landmark  { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence  { get; set; }


        public string? ErrorMessage { get; set; } 
        
    }

    //////////////////////////////////////// part 2 
    #region recommend 
    public class ItineraryResponseDto
    {
        public decimal TotalCost { get; set; }
        public string Currency { get; set; }
        public List<string> OrderedPlaces { get; set; } 
        public Dictionary<string, List<DayScheduleDto>> Schedule { get; set; }
    }
    
    //used
    public class ItineraryRequestDto
    {
        public string City { get; set; }
        public int DurationDays { get; set; }
        public string Category { get; set; }
        public decimal Budget { get; set; }
        public bool IsEgyptian { get; set; }
        public bool IsStudent { get; set; }
        public string Language { get; set; } = "en";
        public int TravelersCount { get; set; } = 1;
    }
    public class DayScheduleDto
    {
        public PlacesResultDTO Place { get; set; }   // 🟢 full DB info instead of just a name
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class AiItineraryResponse
    {
        [JsonPropertyName("total_cost")]
        public decimal TotalCost { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("ordered_places")]
        public List<string>? OrderedPlaces { get; set; }

        [JsonPropertyName("schedule")]
        public Dictionary<string, List<AiDaySchedule>>? Schedule { get; set; }
    }
    //used
    public class AiDaySchedule
    {
        [JsonPropertyName("place_name")]
        public string? PlaceName { get; set; }

        [JsonPropertyName("cost_per_person")] //cost_per_person
        public decimal CostPerPerson { get; set; }

        [JsonPropertyName("start_time")]
        public string? StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public string? EndTime { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success";
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
    #endregion

    public class ChatRequestDto
    {
        public string Message { get; set; }
    }
    public class ChatResponseDto
    {
        public string Response { get; set; }
    }
}
