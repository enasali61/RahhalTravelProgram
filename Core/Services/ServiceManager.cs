using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly Lazy<ITranslationServices> _translationServices;
        private readonly Lazy<IAiService> _aiService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<INotificationService> _notificationService;

        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBasketRepository basketRepository,
            UserManager<Users> userManager,
            IOptions<JwtOptions> options,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment,
            ITranslationServices translationServices,
            ILogger<AiService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration
            )
        {
            _placesService = new Lazy<IPlacesService>(() => new PlacesService(unitOfWork, mapper, webHostEnvironment, translationServices,userManager,httpContextAccessor));
            _categoryService = new Lazy<ICategoryService>(() => new CategoryService(unitOfWork, mapper));
            _tripService = new Lazy<ITripService>(() => new TripService(unitOfWork, mapper, basketRepository,httpContextAccessor,userManager,translationServices));
            _basketService = new Lazy<IBasketService>(() => new BasketService(basketRepository, mapper, unitOfWork, userManager, httpContextAccessor));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, options, configuration, logger));
            _aiService = new Lazy<IAiService>(() => new AiService(httpClientFactory.CreateClient("AI"),logger,unitOfWork,mapper,httpContextAccessor,userManager));
            _userService = new Lazy<IUserService>(() => new UserService(unitOfWork, userManager, httpContextAccessor));
            _notificationService = new Lazy<INotificationService>(() => new NotificationService(unitOfWork, userManager, httpContextAccessor));
        }

        public IPlacesService PlacesService => _placesService.Value;
        public ICategoryService CategoryService => _categoryService.Value;
        public ITripService TripService => _tripService.Value;
        public IBasketService BasketService => _basketService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAiService AiService => _aiService.Value;
        public IUserService UserService => _userService.Value;
        public INotificationService NotificationService => _notificationService.Value;
    }
}
