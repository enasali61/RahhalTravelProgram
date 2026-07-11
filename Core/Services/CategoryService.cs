using System.Numerics;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync();
            var categoriesResult = mapper.Map<IEnumerable<CategoryDto>>(categories);
          
            // return categories
            return categoriesResult;

        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(id);

            // mapping to category dto => imapper
            var categoryResult = mapper.Map<CategoryDto>(category);

            // return category
            return categoryResult;
        }

        public async Task<Dictionary<string, int>> GetPlacesCountByCategoryAsync()
        {
            // retrieve all categories
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync();

            // retrieve all places
            var places = await unitOfWork.GetRepository<Places>().GetAllAsync();

            // create dictionary
            var result = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var count = places.Count(p => p.CategoryId == category.Id);
                result.Add(category.NameAr, count);
            }

            return result;
        }
    }
}
