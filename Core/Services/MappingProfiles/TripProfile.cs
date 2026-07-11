using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.TripAndPlaces;
using Shared.DTOs.PlacesDto;
using Shared.DTOs.TripDto;

namespace Services.MappingProfiles
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<Trip, TripDto>()
            .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => (src.EndDate - src.StartDate).Days + 1))
            .ForMember(dest => dest.BudgetRemaining, opt => opt.MapFrom(src => src.TotalBudget - src.ActualSpent))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<Trip,UpdateTripDto>().ReverseMap();
            CreateMap<Trip,CreateTripDto>().ReverseMap();           
            CreateMap<TripPlace, UpdateTripPlaceDto>().ReverseMap();
            CreateMap<TripPlace, TripPlaceDto>()
            .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place));

        }
    }
}
