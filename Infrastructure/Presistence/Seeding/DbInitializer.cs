using System.Text.Json;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presistence.Data;
using Microsoft.Extensions.Hosting;

namespace Presistence.Seeding
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<Users> _userManager;
        private readonly IHostEnvironment _env;
        public DbInitializer(ApplicationDbContext dbContext,
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<Users> userManager,
             IHostEnvironment env)
        {
            _dbcontext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }
        public async Task InitializeIdentityAsync()
        {
            try
            {
                if (_dbcontext.Database.GetPendingMigrations().Any())
                {
                    await _dbcontext.Database.MigrateAsync();
                }

                if (!_dbcontext.Places.Any())
                {
                    var placesData = @"F:\TravelProgram\Infrastructure\Presistence\Seeding\Rahhal_DataSet_Arabic_Categories_Cities.json";
                    var jsonContent = await File.ReadAllTextAsync(placesData);
                    // Deserialize into DTO with matching field names
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true, // optional, but helps with casing
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString |
                                         System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                    };
                    var placeDtos = JsonSerializer.Deserialize<List<RahhalPlaceDto>>(jsonContent, options);

                    if (placeDtos == null || !placeDtos.Any()) return;

                    // First, ensure categories exist (you already have CategoryId in JSON)
                    // If your Categories table already has these IDs, you can skip creating them.
                    // Otherwise, you may want to seed categories first.

                    var places = new List<Places>();
                    foreach (var dto in placeDtos)
                    {
                        if (string.IsNullOrEmpty(dto.name_ar) && string.IsNullOrEmpty(dto.name_en))
                            continue; // skip invalid entries

                        var place = new Places
                        {
                            NameAr = dto.name_ar ?? "",
                            NameEn = dto.name_en ?? "",
                            PriceEgAdult = dto.price_eg_adult,
                            PriceEgStudent = dto.price_eg_student,
                            PriceForeignAdult = dto.price_forign_adult,
                            PriceForeignStudent = dto.price_forign_student,
                            VisitingTimeEn = dto.VisitingTime ?? "",
                            CityEn = dto.CityEn ?? "",
                            CityAr = dto.CityAr ?? "",
                            CategoryId = dto.CategoryId 
                        };
                        places.Add(place);
                    }

                    await _dbcontext.Places.AddRangeAsync(places);
                    await _dbcontext.SaveChangesAsync();

                }
                await SeedCoordinatesAsync();

                await SeedPlaceImagesAsync();
                await BackfillDescriptionsAsync();
                // seed default user & role 

                // seed roles 
                if (!_roleManager.Roles.Any()) // not contain any role
                {
                    // Admin and user
                    await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole<int>("User"));
                }
                // 2. seed users , assign role for each user 
                if (!_userManager.Users.Any())
                {
                    var adminUser = new Users
                    {
                        UserName = "admin",
                        Email = "Admin@gmail.com",
                        PhoneNumber = "1234567890",
                        UserType = "Admin",
                        InterestsJson = "[]",
                        TravelGroup = "Solo"
                    };
                    var User = new Users
                    {
                        UserName = "Enas",
                        Email = "Enas@gmail.com",
                        PhoneNumber = "1234567890",
                        UserType = "User",
                        InterestsJson = "[]",
                        TravelGroup = "Solo"
                    };
                    await _userManager.CreateAsync(adminUser, "Admin#01");
                    await _userManager.CreateAsync(User, "Enas@li1");
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    await _userManager.AddToRoleAsync(User, "User");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error during seeding: {ex.Message}");
                throw;
            }


        }

        private async Task SeedPlaceImagesAsync()
        {
            try
            {
                Console.WriteLine(" SeedPlaceImagesAsync started.");

                // Get all places from the database
                var places = await _dbcontext.Places.ToListAsync();
                if (!places.Any())
                {
                    Console.WriteLine("No places found in database.");
                    return;
                }

                // Path to the images folder
                var imagesFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "images", "places");
                Console.WriteLine($" Images folder: {imagesFolder}");

                if (!Directory.Exists(imagesFolder))
                {
                    Console.WriteLine($" Folder does not exist: {imagesFolder}");
                    return;
                }

                // Get all image files (any extension: .jfif, .jpg, .jpeg, .webp, .png)
                var imageFiles = Directory.GetFiles(imagesFolder)
                    .Where(f => f.EndsWith(".jfif", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"Found {imageFiles.Count} image files.");

                var addedCount = 0;
                var mainImageAdded = new HashSet<int>();

                foreach (var filePath in imageFiles)
                {
                    // Extract the PlaceId from the file name (e.g., "1" from "1.jfif")
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                    if (!int.TryParse(fileNameWithoutExt, out var placeId))
                    {
                        Console.WriteLine($" Skipping file (invalid ID): {fileNameWithoutExt}");
                        continue;
                    }

                    // Find the place by ID
                    var place = places.FirstOrDefault(p => p.Id == placeId);
                    if (place == null)
                    {
                        Console.WriteLine($" No place found for ID: {placeId}");
                        continue;
                    }

                    var fileExt = Path.GetExtension(filePath); // e.g., ".jfif"
                    var imageUrl = $"/images/places/{fileNameWithoutExt}{fileExt}";

                    // Check if this image is already linked
                    var existing = await _dbcontext.PlaceImages
                        .FirstOrDefaultAsync(pi => pi.PlaceId == placeId && pi.ImageUrl == imageUrl);
                    if (existing != null)
                    {
                        Console.WriteLine($"Image already exists for PlaceId {placeId}");
                        continue;
                    }

                    // Determine if this should be the main image (first one found)
                    var isMain = !mainImageAdded.Contains(placeId);

                    var placeImage = new PlaceImages
                    {
                        PlaceId = placeId,
                        ImageUrl = imageUrl,
                        IsMain = isMain,
                        DisplayOrder = mainImageAdded.Contains(placeId) ? 1 : 0
                    };

                    await _dbcontext.PlaceImages.AddAsync(placeImage);
                    if (isMain)
                        mainImageAdded.Add(placeId);

                    addedCount++;
                    Console.WriteLine($"Added image for PlaceId {placeId}: {imageUrl} (Main: {isMain})");
                }

                await _dbcontext.SaveChangesAsync();
                Console.WriteLine($"Seeding complete. Added {addedCount} images.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding images: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        private async Task BackfillDescriptionsAsync()
        {
            var placesData = @"F:\TravelProgram\Infrastructure\Presistence\Seeding\Rahhal_DataSet_Arabic_Categories_Cities.json";
            var jsonContent = await File.ReadAllTextAsync(placesData);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var placeDtos = JsonSerializer.Deserialize<List<RahhalPlaceDto>>(jsonContent, options);

            if (placeDtos == null) return;

            foreach (var dto in placeDtos)
            {
                if (string.IsNullOrEmpty(dto.DescriptionEn)) continue;

                var existing = await _dbcontext.Places
                    .FirstOrDefaultAsync(p => p.NameEn == dto.name_en || p.NameAr == dto.name_ar);

                if (existing != null && string.IsNullOrEmpty(existing.HistoricalBackGroundEn))
                {
                    existing.HistoricalBackGroundEn = dto.DescriptionEn;
                }
            }

            await _dbcontext.SaveChangesAsync();
        }

        private async Task SeedCoordinatesAsync()
        {
            
            var jsonPath = @"F:\TravelProgram\Infrastructure\Presistence\Seeding\map.json";
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine(" Coordinates file not found.");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var coordinates = JsonSerializer.Deserialize<List<PlaceCoordinateDto>>(json);

            if (coordinates == null || !coordinates.Any())
                return;

            foreach (var coord in coordinates)
            {
                var place = await _dbcontext.Places.FirstOrDefaultAsync(p => p.Id == coord.Id);
                if (place != null)
                {
                    place.Latitude = coord.Latitude;
                    place.Longitude = coord.Longitude;
                    Console.WriteLine($" Updated Place {coord.Id}: ({coord.Latitude}, {coord.Longitude})");
                }
                else
                {
                    Console.WriteLine($"Place {coord.Id} not found.");
                }
            }

            await _dbcontext.SaveChangesAsync();
            Console.WriteLine($"Coordinates seeded for {coordinates.Count} places.");
        }

        // DTO for deserialization
        public class PlaceCoordinateDto
        {
            public int Id { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
    public class RahhalPlaceDto
    {
        public string name_en { get; set; }
        public decimal price_eg_adult { get; set; }
        public decimal price_eg_student { get; set; }
        public decimal price_forign_adult { get; set; }
        public decimal price_forign_student { get; set; }
        public string VisitingTime { get; set; }
        public string? DescriptionEn { get; set; }
        public int CategoryId { get; set; }
        public string CityEn { get; set; }
        public string name_ar { get; set; }
        public string CityAr { get; set; }

    }
}
