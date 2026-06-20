using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepo;

        public CategoriesController(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<Category>> { Success = true, Message = "Categories retrieved.", Data = categories });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new ApiResponse<Category> { Success = true, Message = "Category created.", Data = category });
        }
    }
}
