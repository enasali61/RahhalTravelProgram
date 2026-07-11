using Domain;
using Domain.Entities.TripAndPlaces;
using Shared;

namespace Services.Specifications
{

    internal class TripSpec : Specifications<Trip>
    {
        public TripSpec(int id) : base(t => t.Id == id)
        {
            AddInclude(t => t.TripPlaces);
            AddInclude(t => t.User);
            // If you need places inside each TripPlace:
            // AddInclude(t => t.TripPlaces.Select(tp => tp.Place));
        }

        // For GetAll (without a filter, but with includes)
        public TripSpec(TripSpecParams specParams)
       : base(trip =>
           (!specParams.IsTemplate.HasValue || trip.IsTemplate == specParams.IsTemplate) &&
           (!specParams.UserId.HasValue || trip.UserId == specParams.UserId) &&
           (string.IsNullOrEmpty(specParams.Search)
        || trip.TripName.ToLower().Contains(specParams.Search.ToLower().Trim())))
        {
            // Include related entities (TripPlaces, User, and Place inside TripPlaces)
            AddInclude(t => t.TripPlaces);
            AddInclude(t => t.User);
            //  AddInclude(t => t.TripPlaces.Select(tp => tp.Place)); // if you need place details in the trip DTO

            // Sorting
            if (specParams.Sort.HasValue)
            {
                switch (specParams.Sort.Value)
                {
                    case TripSortOption.NameAsc:
                        SetOrderBy(t => t.TripName);
                        break;
                    case TripSortOption.NameDesc:
                        SetOrderByDescending(t => t.TripName);
                        break;
                    case TripSortOption.StartDateAsc:
                        SetOrderBy(t => t.StartDate);
                        break;
                    case TripSortOption.StartDateDesc:
                        SetOrderByDescending(t => t.StartDate);
                        break;
                    case TripSortOption.BudgetAsc:
                        SetOrderBy(t => t.TotalBudget);
                        break;
                    case TripSortOption.BudgetDesc:
                        SetOrderByDescending(t => t.TotalBudget);
                        break;
                    case TripSortOption.StatusAsc:
                        SetOrderBy(t => t.Status);
                        break;
                    case TripSortOption.StatusDesc:
                        SetOrderByDescending(t => t.Status);
                        break;
                    default:
                        SetOrderBy(t => t.Id);
                        break;
                }
            }

            // Pagination
            ApplyPagination(specParams.PageIndex, specParams.PageSize);
        }
    }
}
