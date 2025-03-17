using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Services;
using DTOs;

namespace Controllers;

[ApiController]
[Route("api/user")]
public class UserController(
    UserManager<User> userManager, 
    SignInManager<User> signInManager, 
    AuthService authService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User { UserName = request.Username, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded is false)
        {
            return BadRequest(result.Errors);
        }

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded is false)
        {
            return Unauthorized("Invalid username or password");
        }

        var token = authService.GenerateJwtToken(user);
        return Ok(new LoginResponse(token));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok("Logged out");
    }
}
