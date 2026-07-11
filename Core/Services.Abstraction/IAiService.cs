using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.DTOs.TripDto;

namespace Services.Abstraction
{
    public interface IAiService
    {
        Task<AiRecognitionResult> RecognizeImageAsync(Stream imageStream, string fileName);
        Task<TripDto> GenerateItineraryAsync(ItineraryRequestDto request, string? language = null);
        Task<string> SendChatMessageAsync(ChatRequestDto request, string message); //  New method

    }
}
