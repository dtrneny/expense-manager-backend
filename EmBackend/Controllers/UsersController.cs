using EmBackend.Entities;
using EmBackend.Repositories;
using EmBackend.Repositories.Users;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    
    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
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
}