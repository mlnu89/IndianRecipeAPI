using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using IndianRecipeAPI.Models;
using IndianRecipeAPI.Repositories;
using System.Threading.Tasks;

namespace IndianRecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Ensure this is added to secure all endpoints in this controller
    public class RecipesController : ControllerBase
    {
        private readonly IRepository<Recipe> _recipeRepository;

        public RecipesController(IRepository<Recipe> recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        // GET: api/recipes
        [HttpGet]
        public async Task<IActionResult> GetRecipes(string? region, string? dietType)
        {
            // If no filters are passed, return all recipes
            if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(dietType))
            {
                var recipes = await _recipeRepository.GetAllAsync();
                return Ok(recipes);
            }

            // Build filter query for region and/or dietType
            var filter = Builders<Recipe>.Filter.Empty;  // Start with an empty filter

            if (!string.IsNullOrEmpty(region))
                filter &= Builders<Recipe>.Filter.Eq(r => r.Region, region);

            if (!string.IsNullOrEmpty(dietType))
                filter &= Builders<Recipe>.Filter.Eq(r => r.DietType, dietType);

            var filteredRecipes = await _recipeRepository.GetByFilterAsync(filter);
            return Ok(filteredRecipes);
        }

        // Other CRUD actions...
    }
}
