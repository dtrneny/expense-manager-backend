using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Mappers;
using EmBackend.Models.Helpers;
using EmBackend.Models.Users.Requests;
using EmBackend.Models.Users.Responses;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
using EmBackend.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class UsersController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly EntityMapper _entityMapper;
    private readonly ModelValidation _modelValidation;
    public UsersController(
        IRepository<User> userRepository,
        EntityMapper entityMapper,
        ModelValidation modelValidation
    )
    {
        _userRepository = userRepository;
        _entityMapper = entityMapper;
        _modelValidation = modelValidation;
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
        
        var emailFilter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Email, data.Email)
        );
        if (emailFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        var users = await _userRepository.GetAll(emailFilter);
        if (users.Any()) { return BadRequest("Provided email is unavailable."); }

        var userValidationResult = _modelValidation.UserValidator.Validate(userData);
        if (userValidationResult == null) { return StatusCode(500); }
        if (!userValidationResult.IsValid) { return BadRequest(userValidationResult.Errors); }
        
        var user = await _userRepository.Create(userData);
        if (user == null) { return Problem("User could not be created."); }

        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return StatusCode(500); }

        return Ok(new PostUserResponse(userDto));
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser(UpdateUserRequest data, string id)
    {
        var idValidationResult = _modelValidation.ObjectIdValidator.Validate(id);
        if (idValidationResult == null) { return StatusCode(500); }
        if (!idValidationResult.IsValid) { return BadRequest(idValidationResult.Errors); }
        
        var updateValidationResult = _modelValidation.UpdateUserValidator.Validate(data);
        if (updateValidationResult == null) { return StatusCode(500); }
        if (!updateValidationResult.IsValid) { return BadRequest(updateValidationResult.Errors); }
        
        var changesDocument = BsonUtility.ToBsonDocument(data);
        var update = MongoDbDefinitionBuilder.BuildUpdateDefinition<User>(changesDocument);
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Id, id)
        );
        if (filter == null || update == null) { return BadRequest("The provided data could not be utilized for filter or update."); }
        
        var user = await _userRepository.Update(update, filter);
        if (user == null) { return Problem("User could not be updated."); }
        
        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return StatusCode(500); }

        return Ok(new UpdateUserResponse(userDto));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUsersResponse>> GetUser(string id)
    {
        var idValidationResult = _modelValidation.ObjectIdValidator.Validate(id);
        if (idValidationResult == null) { return StatusCode(500); }
        if (!idValidationResult.IsValid) { return BadRequest(idValidationResult.Errors); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Id, id)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var user = await _userRepository.GetOne(filter);
        if (user == null) { return NotFound(); }

        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return StatusCode(500); }

        return Ok(new GetUserResponse(userDto));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers()
    {
        var result = await _userRepository.GetAll();
        return Ok(result.ToList());
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageResponse>> DeleteUser(string id)
    {
        var idValidationResult = _modelValidation.ObjectIdValidator.Validate(id);
        if (idValidationResult == null) { return StatusCode(500); }
        if (!idValidationResult.IsValid) { return BadRequest(idValidationResult.Errors); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Id, id)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var deleteResult = await _userRepository.Delete(filter);
        if (deleteResult == null) { return Problem("User could not be deleted."); }
        if (deleteResult.DeletedCount == 0) { return BadRequest("User could not be deleted."); }

        return Ok("User was deleted.");
    }
}