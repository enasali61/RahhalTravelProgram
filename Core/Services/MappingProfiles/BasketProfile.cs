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
    internal class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<CustomerBasket, BasketDto>().ReverseMap();
            CreateMap<BasketItems, BasketItemsDto>()
                    .ForMember(dest => dest.Name, opt => opt.Ignore())
                    .ForMember(dest => dest.Description, opt => opt.Ignore())
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceSnapshot)).ReverseMap();
            
        }
    }
}
