using EmBackend.Entities;
using EmBackend.Models.Categories.Responses;
using EmBackend.Models.Users.Requests;
using EmBackend.Models.Users.Responses;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly EntityMapper _entityMapper;
    private readonly Validation _validation;
    public UsersController(
        IRepository<User> userRepository,
        EntityMapper entityMapper,
        Validation validation
    )
    {
        _userRepository = userRepository;
        _entityMapper = entityMapper;
        _validation = validation;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<PostUserResponse>> PostUser(PostUserRequest data)
    {
        var userData = new User {
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            Email = data.Email,
            Password = data.Password
        };
        
        var emailFilter = EntityOperationBuilder<User>.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Email, data.Email)
        );
        if (emailFilter == null) { return StatusCode(500); }
        var users = await _userRepository.GetAll(emailFilter);
        if (users.Any()) { return BadRequest(); }

        var userValidationResult = _validation.UserValidator.Validate(userData);
        if (userValidationResult == null) { return StatusCode(500); }
        if (!userValidationResult.IsValid) { return BadRequest(userValidationResult.Errors); }
        
        var user = await _userRepository.Create(userData);
        if (user == null) { return StatusCode(500); }

        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return StatusCode(500); }

        return Ok(new PostUserResponse(userDto));
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser(UpdateUserRequest data, string id)
    {
        var updateValidationResult = _validation.UpdateUserValidator.Validate(data);
        if (updateValidationResult == null) { return StatusCode(500); }
        if (!updateValidationResult.IsValid) { return BadRequest(updateValidationResult.Errors); }
        
        var changesDocument = BsonUtilities.ToBsonDocument(data);

        var update = EntityOperationBuilder<User>.BuildUpdateDefinition(changesDocument);
        var filter = EntityOperationBuilder<User>.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Id, id)
        );
        if (filter == null || update == null) { return BadRequest();}
        
        var user = await _userRepository.Update(update, filter);
        if (user == null) { return StatusCode(500); }
        
        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return BadRequest(); }

        return Ok(new UpdateUserResponse(userDto));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUsersResponse>> GetUser(string id)
    {
        var filter = EntityOperationBuilder<User>.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Id, id)
        );
        if (filter == null) { return BadRequest();}
        
        var user = await _userRepository.GetOne(filter);
        if (user == null) { return BadRequest();}

        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return BadRequest(); }

        return Ok(new GetUserResponse(userDto));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers()
    {
        var result = await _userRepository.GetAll();
        return Ok(result.ToList());
    }
}