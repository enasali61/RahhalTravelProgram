using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<Dictionary<string, int>> GetPlacesCountByCategoryAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);

    }
}
