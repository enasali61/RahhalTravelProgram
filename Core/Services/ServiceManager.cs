using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Services.Abstraction;
using Shared;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPlacesService> _placesService;
        private readonly Lazy<ICategoryService> _categoryService;
        private readonly Lazy<ITripService> _tripService;
        private readonly Lazy<IBasketService> _basketService;
        private readonly Lazy<IAuthenticationService> _authenticationService;



        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBasketRepository basketRepository,
            UserManager<Users> userManager,
            IOptions<JwtOptions> options
            )
        {
            _placesService = new Lazy<IPlacesService>(() => new PlacesService(unitOfWork, mapper));
            _categoryService = new Lazy<ICategoryService>(() => new CategoryService(unitOfWork, mapper));
            _tripService = new Lazy<ITripService>(() => new TripService(unitOfWork, mapper));
            _basketService = new Lazy<IBasketService>(() => new BasketService(basketRepository, mapper, unitOfWork));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager,options));
        }

        public IPlacesService PlacesService => _placesService.Value;
        public ICategoryService CategoryService => _categoryService.Value;
        public ITripService TripService => _tripService.Value;
        public IBasketService BasketService => _basketService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
