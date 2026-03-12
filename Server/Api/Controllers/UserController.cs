using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO userDto)
    {
        try
        {
            var user = await userService.CreateUserAsync(userDto);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }
}