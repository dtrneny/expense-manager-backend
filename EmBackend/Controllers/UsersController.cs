using EmBackend.Entities;
using EmBackend.Repositories;
using EmBackend.Repositories.Auth;
using EmBackend.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly AuthRepository _authRepository;
    
    public UsersController(IRepository<User> userRepository, AuthRepository authRepository)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<User?>> CreateUser(User user)
    {
        var result = await _userRepository.Create(user);

        if (user == null)
        {
            return Ok("Not so ok");
        }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var result = await _userRepository.GetAll();
        return Ok(result);
    }
}