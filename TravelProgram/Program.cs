using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presistence.Data;
using Presistence.Repositories;
using Services;
using Services.Abstraction;
using TravelProgram.Extentions;
using TravelProgram.Factories;
using TravelProgram.MiddelWares;

namespace TravelProgram
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Services

            builder.Services.AddCoreServices(builder.Configuration);

            builder.Services.AddPresentationServices();

            builder.Services.AddInfrastructureService(builder.Configuration);
                     
            #endregion

            var app = builder.Build();
            await InitializeDbAsync(app);
            #region Configure Kesterl Middleware
            app.UseCustomExceptionMiddelWare();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            #endregion

          

            app.Run();
           async Task InitializeDbAsync(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var dbInitalizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitalizer.InitializeIdentityAsync();
            }
        }
    }
}
