using EmBackend.Entities;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Movements.Responses;
using EmBackend.Repositories;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MovementsController: ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly AuthRepository _authRepository;
    private readonly EntityMapper _entityMapper;
    private readonly Validation _validation;
    
    public MovementsController(
        IRepository<Movement> movementRepository,
        AuthRepository authRepository,
        Validation validation,
        EntityMapper entityMapper
    )
    {
        _movementRepository = movementRepository;
        _authRepository = authRepository;
        _validation = validation;
        _entityMapper = entityMapper;
    }

    [HttpPost]
    public async Task<ActionResult<PostMovementResponse>> PostMovement(PostMovementRequest data)
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);

        if (userId == null) { return Unauthorized(); }
        
        var movement = new Movement {
            UserId = userId,
            Amount = data.Amount,
            Label = data.Label,
            CategoryIds = data.CategoryIds
        };
        
        var result = await _movementRepository.Create(movement);
    
        if (result?.Id == null) { return StatusCode(500); }

        var movementDto = _entityMapper.MovementMapper.MapMovementToMovementDto(result);
        
        var validationResult = _validation.MovementDtoValidator.Validate(movementDto);
        if (validationResult == null) { return StatusCode(500); }
        if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }
        
        return Ok(new PostMovementResponse(movementDto));
    }
}