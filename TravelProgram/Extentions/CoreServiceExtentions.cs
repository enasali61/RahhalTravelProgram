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
            services.AddHttpClient<AiService>("AI", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30); // adjust based on friend's API speed
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true; // ✅ Bypass SSL
                return handler;
            }); ;
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()   // يسمح لأي نطاق
                              .AllowAnyMethod()   // GET, POST, PUT, DELETE
                              .AllowAnyHeader();  // أي Header
                    });
            });

            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            return services;

        }
    }
}
