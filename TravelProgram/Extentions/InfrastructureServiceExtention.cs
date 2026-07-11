using System.Text;
using Domain.Contracts;
using Domain.Entities;
using GTranslate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presistence.Data;
using Presistence.Repositories;
using Presistence.Seeding;
using SendGrid;
using Services;
using Services.Abstraction;
using Services.Translation;
using Shared;
using StackExchange.Redis;


namespace TravelProgram.Extentions
{
    public static class InfrastructureServiceExtention
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!)
                );
            services.AddScoped<ISendGridClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new SendGridClient(config["SendGrid:ApiKey"]);
            }); services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ITranslationServices, GTranslateTranslationService>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUserService, UserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.ConfigureIdentityService();
            services.ConfigureJwt(configuration);
            return services;
        }

        public static IServiceCollection ConfigureIdentityService(this IServiceCollection services)
        {
            services.AddIdentity<Users, IdentityRole<int>>(
                option => {
                    option.Password.RequireDigit = true;
                    option.Password.RequireUppercase = true;
                    option.User.RequireUniqueEmail = true;
                    option.Password.RequiredLength = 5;
                }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders(); ;
            return services;
        }

        public static IServiceCollection ConfigureJwt(
            this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            // validate on token 
            services.AddAuthentication(

                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        // values
                        ValidAudience = jwtOptions.Audience,
                        ValidIssuer = jwtOptions.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))

                    };
                });
            services.AddAuthorization();
            return services;

        }
    }
}
