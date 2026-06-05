using Domain.Contracts;
using Presistence.Data;
using TravelProgram.MiddelWares;

namespace TravelProgram.Extentions
{
    public static class WebApplicationExtintions
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbIntializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await dbIntializer.InitializeIdentityAsync();
            return app;
        }

        public static async Task<WebApplication> UseCustomExceptionMiddelWare(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandelMiddelWare>();

            return app;
        }
    }
}
