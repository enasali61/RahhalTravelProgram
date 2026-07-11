using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;

namespace Presentation
{
  
    public class CategoriesController(IServiceManager serviceManager) : BaseApiController
    {
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await serviceManager.CategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await serviceManager.CategoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

      
        // GET: api/Categories/stats
        [HttpGet("stats")]
        public async Task<ActionResult<Dictionary<string, int>>> GetPlacesCountByCategory()
        {
            var stats = await serviceManager.CategoryService.GetPlacesCountByCategoryAsync();
            return Ok(stats);
        }


    }

}
