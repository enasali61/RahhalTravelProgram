using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Shared.DTOs;
using Shared.DTOs.PlacesDto;

namespace Services.MappingProfiles
{
    public class PlacesProfile : Profile
    {
       
        public PlacesProfile()
        {
            CreateMap<PlaceImages, PlaceImagesDto>();
            CreateMap<Places,UpdatePlaceDto>().ReverseMap();
            CreateMap<Places,CreatePlaceDto>().ReverseMap();
            CreateMap<Places, PlacesResultDTO>()
               .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom<PictureUrlResolver>())
               .ForMember(dest => dest.GalleryImages,
                   opt => opt.MapFrom(src =>
                       src.Images.Where(img => !img.IsMain)
                                 .OrderBy(img => img.DisplayOrder)
                                 .Select(img => img.ImageUrl)
                                 .ToList()))
               .ForMember(dest => dest.HistoricalBackGround,
                opt => opt.MapFrom(src => src.HistoricalBackGroundEn))
               .ForMember(dest => dest.IsFavorite, opt => opt.Ignore())
               .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src =>
                    src.Category != null ? src.Category.NameEn : string.Empty)); // هتحسبيه في الـ Service   

            CreateMap<Category, CategoryDto>();


        }
    }
}