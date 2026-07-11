using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities.TripAndPlaces;
using Shared;

namespace Services.Specifications
{
    internal class PlacesWithImageSpec : Specifications<Places>
    {
        public PlacesWithImageSpec(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Images);
        }
        public PlacesWithImageSpec(PlacesSpecParams placesSpecParams) :
            base(place =>
            (!placesSpecParams.CategoryId.HasValue || place.CategoryId == placesSpecParams.CategoryId)
            &&
            (string.IsNullOrEmpty(placesSpecParams.Search) || place.NameEn.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.NameAr.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.CityEn.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.CityAr.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            ))
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Images);
            
            if (placesSpecParams.Sort is not null)
            {
                // sorting
                switch(placesSpecParams.Sort)
                {
                    case PlacesSortOption.NameEnglishAsc:
                        SetOrderBy(p => p.NameEn);
                        break;
                    case PlacesSortOption.NameEnglishDesc:
                        SetOrderByDescending(p => p.NameEn);
                        break;
                    case PlacesSortOption.RatingAsc:
                        SetOrderBy(p => p.Rating);
                        break;
                    case PlacesSortOption.RatingDesc:
                        SetOrderByDescending(p => p.Rating);
                        break;
                    case PlacesSortOption.NameArabicAsc:
                        SetOrderBy(p => p.NameAr);
                        break;
                    case PlacesSortOption.NameArabicDesc:
                        SetOrderByDescending(p => p.NameAr);
                        break;
                    default:
                        SetOrderBy(p => p.Id);
                        break;
                }

            }


            ApplyPagination(placesSpecParams.PageIndex, placesSpecParams.PageSize);

        }

        
    }
}
