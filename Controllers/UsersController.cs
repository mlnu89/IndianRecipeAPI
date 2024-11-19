using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IndianRecipeAPI.Models;
using IndianRecipeAPI.Repositories;

namespace IndianRecipeAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepository;

    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { Message = "User not found." });

        return Ok(new
        {
            Id = user.Id,
            Username = user.Username,
            DietaryPreference = user.DietaryPreference,
            Message = "User profile retrieved successfully."
        });
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            return NotFound(new { Message = "User not found." });

        if (!string.IsNullOrEmpty(updatedUser.Username))
            existingUser.Username = updatedUser.Username;

        if (!string.IsNullOrEmpty(updatedUser.DietaryPreference))
            existingUser.DietaryPreference = updatedUser.DietaryPreference;

        await _userRepository.UpdateAsync(id, existingUser);

        return Ok(new
        {
            Id = existingUser.Id,
            Username = existingUser.Username,
            DietaryPreference = existingUser.DietaryPreference,
            Message = "User profile updated successfully."
        });
    }
}
