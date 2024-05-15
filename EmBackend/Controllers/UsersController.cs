using EmBackend.Entities;
using EmBackend.Models.Users.Requests;
using EmBackend.Models.Users.Responses;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
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
    public async Task<ActionResult<PostUserResponse>> PostUser(PostUserRequest data)
    {
        var user = new User {
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            Email = data.Email,
            Password = data.Password
        };
        
        var result = await _userRepository.Create(user);

        if (result == null) { return StatusCode(500); }

        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPatch]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser(UpdateUserRequest data)
    {
        var changesDocument = BsonUtilities.ToBsonDocument(data);

        var update = EntityOperationBuilder<User>.BuildUpdateDefinition(changesDocument);
        
        var filter = EntityOperationBuilder<User>.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Id, data.Id)
        );
        
        if (filter == null || update == null) { return BadRequest();}
        
        var result = await _userRepository.Update(update, filter);
        
        if (result == null) { return StatusCode(500); }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers()
    {
        var result = await _userRepository.GetAll();
        return Ok(result.ToList());
    }
}