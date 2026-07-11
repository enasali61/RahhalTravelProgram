using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities.TripAndPlaces;
using Shared;

namespace Services.Specifications
{
    public class PlacesCountSpec : Specifications<Places>
    {
        public PlacesCountSpec(PlacesSpecParams placesSpecParams) :
            base(place => (!placesSpecParams.CategoryId.HasValue || place.CategoryId == placesSpecParams.CategoryId) &&
            (string.IsNullOrEmpty(placesSpecParams.Search) || place.NameEn.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.NameAr.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.CityEn.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())
            || place.CityAr.ToLower().Contains(placesSpecParams.Search.ToLower().Trim())

            ))
            
        {
           

        }
    }
}
