using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FruitsController : ControllerBase
    {
        private readonly IRepository<Fruit> _fruitRepo;

        public FruitsController(IRepository<Fruit> fruitRepo)
        {
            _fruitRepo = fruitRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fruits = await _fruitRepo.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<Fruit>> { Success = true, Message = "Fruits retrieved.", Data = fruits });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var fruit = await _fruitRepo.GetByIdAsync(id);
            if (fruit == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Fruit not found." });

            return Ok(new ApiResponse<Fruit> { Success = true, Message = "Fruit details returned.", Data = fruit });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] Fruit fruit)
        {
            await _fruitRepo.AddAsync(fruit);
            await _fruitRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = fruit.Id }, new ApiResponse<Fruit> { Success = true, Message = "Fruit created.", Data = fruit });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Fruit fruit)
        {
            var existing = await _fruitRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Fruit not found." });

            existing.Name = fruit.Name;
            existing.Description = fruit.Description;
            existing.Price = fruit.Price;
            existing.DiscountPrice = fruit.DiscountPrice;
            existing.Stock = fruit.Stock;
            existing.Category = fruit.Category;
            existing.ImageUrl = fruit.ImageUrl;
            if (!string.IsNullOrWhiteSpace(fruit.ImageData))
            {
                existing.ImageData = fruit.ImageData;
            }
            existing.IsFeatured = fruit.IsFeatured;
            existing.Status = fruit.Status;
            existing.PriceUnit = fruit.PriceUnit;
            _fruitRepo.Update(existing);
            await _fruitRepo.SaveChangesAsync();

            return Ok(new ApiResponse<Fruit> { Success = true, Message = "Fruit updated.", Data = existing });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _fruitRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Fruit not found." });

            _fruitRepo.Remove(existing);
            await _fruitRepo.SaveChangesAsync();
            return Ok(new ApiResponse<string> { Success = true, Message = "Fruit deleted." });
        }
    }
}
