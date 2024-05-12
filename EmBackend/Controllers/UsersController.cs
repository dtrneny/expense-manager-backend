using EmBackend.Entities;
using EmBackend.Payloads.Users;
using EmBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    
    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(User user)
    {
        var result = await _userRepository.Create(user);

        if (user == null) { return StatusCode(500); }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<UsersResponse>> GetUsers()
    {
        var result = await _userRepository.GetAll();
        return Ok(result.ToList());
    }
}