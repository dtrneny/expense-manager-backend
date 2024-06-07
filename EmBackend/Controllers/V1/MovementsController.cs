using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Mappers;
using EmBackend.Models.Movements.Params;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Movements.Responses;
using EmBackend.Models.Users.Requests;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class MovementsController: ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly IRepository<User> _userRepository;
    private readonly AuthRepository _authRepository;
    private readonly EntityMapper _entityMapper;
    private readonly Validation.Validation _validation;
    
    public MovementsController(
        IRepository<Movement> movementRepository,
        IRepository<User> userRepository,
        AuthRepository authRepository,
        Validation.Validation validation,
        EntityMapper entityMapper
    )
    {
        _movementRepository = movementRepository;
        _userRepository = userRepository;
        _authRepository = authRepository;
        _validation = validation;
        _entityMapper = entityMapper;
    }

    [HttpPost]
    public async Task<ActionResult<PostMovementResponse>> PostMovement(PostMovementRequest data)
    {
        var userFilter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Id, data.UserId)
        );
        if (userFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var user = await _userRepository.GetOne(userFilter);
        if (user == null) { return NotFound("User could not be found."); }
        
        var movementData = new Movement {
            UserId = data.UserId,
            Amount = data.Amount,
            Label = data.Label,
            Timestamp = data.Timestamp,
            CategoryIds = data.CategoryIds
        };
        
        var movementValidationResult = _validation.MovementValidator.Validate(movementData);
        if (movementValidationResult == null) { return StatusCode(500); }
        if (!movementValidationResult.IsValid) { return BadRequest(movementValidationResult.Errors); }

        var updateRequest = new UpdateUserRequest(null, null, null, Balance: user.Balance + data.Amount);
        var changesDocument = BsonUtility.ToBsonDocument(updateRequest);
        var update = MongoDbDefinitionBuilder.BuildUpdateDefinition<User>(changesDocument);
        if (update == null) { return BadRequest("The provided data could not be utilized for update."); }
        
        var movement = await _movementRepository.Create(movementData);
        if (movement == null) { return Problem("Movement could not be created."); }
        
        var userUpdate = await _userRepository.Update(update, userFilter);
        if (userUpdate == null) { return Problem("User balance could not be updated."); }
        
        var movementDto = _entityMapper.MovementMapper.MapMovementToMovementDto(movement);
        if (movementDto == null) { return StatusCode(500); }
        
        return Ok(new PostMovementResponse(movementDto));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetMovementsResponse>> GetMovements(GetMovementsParams queryParams)
    {
        var query = MongoDbDefinitionBuilder.BuildFilterDefinitionFromQuery<Movement, GetMovementsParams>(queryParams);
        if (query == null) { return BadRequest("The provided data could not be utilized for query."); }
        
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Movement>(builder =>
            builder.Eq(movement => movement.UserId, userId)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var combinedFilter = query & filter;

        var movements = await _movementRepository.GetAll(combinedFilter);
        var movementsDtos = movements
            .Select(movement => _entityMapper.MovementMapper.MapMovementToMovementDto(movement))
            .ToList();
        
        return Ok(new GetMovementsResponse(movementsDtos));
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateMovementResponse>> UpdateMovement(UpdateMovementRequest data, string id)
    {
        var updateValidationResult = _validation.UpdateMovementValidator.Validate(data);
        if (updateValidationResult == null) { return StatusCode(500); }
        if (!updateValidationResult.IsValid) { return BadRequest(updateValidationResult.Errors); }
        
        var changesDocument = BsonUtility.ToBsonDocument(data);
        var update = MongoDbDefinitionBuilder.BuildUpdateDefinition<Movement>(changesDocument);
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Movement>(builder =>
            builder.Eq(category => category.Id, id)
        );
        if (filter == null || update == null) { return BadRequest("The provided data could not be utilized for filter or update."); }
        
        var movement = await _movementRepository.Update(update, filter);
        if (movement == null) { return Problem("Movement could not be updated."); }
        
        var movementDto = _entityMapper.MovementMapper.MapMovementToMovementDto(movement);
        if (movementDto == null) { return StatusCode(500); }

        return Ok(new UpdateMovementResponse(movementDto));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovement(string id)
    {
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Movement>(builder =>
            builder.Eq(movement => movement.Id, id)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var deleteResult = await _movementRepository.Delete(filter);
        if (deleteResult == null) { return Problem("Movement could not be updated."); }

        return Ok("Movement deleted.");
    }
}