using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstraction
{
    public interface IServiceManager
    {
        // signiture for every service
        public IPlacesService PlacesService { get; }
        public ICategoryService CategoryService { get; }
        public ITripService TripService { get; }
        public IBasketService BasketService { get; }
        public IAuthenticationService AuthenticationService { get; }

    }
}
