using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Shared.DTOs;

namespace Services.MappingProfiles
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<Trip, TripDto>()
            .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => (src.EndDate - src.StartDate).Days + 1))
            .ForMember(dest => dest.BudgetRemaining, opt => opt.MapFrom(src => src.TotalBudget - src.ActualSpent));
            
            CreateMap<TripPlace, TripPlaceDto>()
            .ForMember(dest => dest.Places, opt => opt.MapFrom(src => src.Place));

        }
    }
}
