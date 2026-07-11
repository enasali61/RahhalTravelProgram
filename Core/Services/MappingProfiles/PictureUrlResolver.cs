using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Execution;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.DTOs.PlacesDto;

namespace Services.MappingProfiles
{
    internal class PictureUrlResolver(IConfiguration configuration) : IValueResolver<Places, PlacesResultDTO, string>
    {    
        public string Resolve(Places source, PlacesResultDTO destination, string destMember, ResolutionContext context)
        {
        
          // Get the main image (or the first one if no main is set)
            var image = source.Images?.FirstOrDefault(i => i.IsMain) ?? source.Images?.FirstOrDefault();

            if (string.IsNullOrEmpty(image?.ImageUrl))
                return string.Empty;

            // If it's already an absolute URL, return it as is.
            if (image.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return image.ImageUrl;
            // https://implant-liberty-transfer.ngrok-free.dev
            return $"{configuration["ImageBaseUrl"]}{image.ImageUrl}";
        }
    }
}
