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
    public class PlacesProfile : Profile
    {
        public PlacesProfile()
        {
            CreateMap<PlaceImages, PlaceImagesDto>();

            CreateMap<Places, PlacesResultDTO>()
               .ForMember(dest => dest.MainImageUrl,
                   opt =>
                   opt.MapFrom(src =>
                       src.Images.FirstOrDefault(img => img.IsMain)!.ImageUrl))
               .ForMember(dest => dest.GalleryImages,
                   opt => opt.MapFrom(src =>
                       src.Images.Where(img => !img.IsMain)
                                 .OrderBy(img => img.DisplayOrder)
                                 .Select(img => img.ImageUrl)
                                 .ToList()))
               .ForMember(dest => dest.IsFavorite,
                   opt => opt.Ignore())
               .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src =>
                    src.Category != null ? src.Category.NameAr : string.Empty)); // هتحسبيه في الـ Service

      

        }
    }
}