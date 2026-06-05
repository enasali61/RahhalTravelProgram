using Services;
using Services.Abstraction;
using Shared;

namespace TravelProgram.Extentions
{
    public static class CoreServiceExtentions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { }, typeof(Services.AssemblyRefrence).Assembly);
            services.AddScoped<IServiceManager, ServiceManager>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            return services;

        }
    }
}
