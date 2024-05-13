using EmBackend.Entities;
using EmBackend.Models.Movements;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Movements.Responses;
using EmBackend.Repositories;
using EmBackend.Repositories.Auth;
using EmBackend.Utilities;
using EmBackend.Utilities.Mappers;
using FluentValidation;
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
    private readonly MovementMapper _movementMapper;
    private readonly Validation _validation;
    
    public MovementsController(IRepository<Movement> movementRepository, AuthRepository authRepository, Validation validation)
    {
        _movementRepository = movementRepository;
        _authRepository = authRepository;
        _movementMapper = new MovementMapper();
        _validation = validation;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostMovementResponse>> PostMovement(PostMovementRequest data)
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);

        if (userId == null) { return Unauthorized(); }
        
        var movement = new Movement {
            UserId = userId,
            Amount = data.Amount,
            Label = data.Label
        };
        
        var result = await _movementRepository.Create(movement);
    
        if (result?.Id == null) { return StatusCode(500); }

        var movementDto = _movementMapper.MovementToMovementDto(result);
        
        var validationTask = _validation.MovementDtoValidator.ValidateAsync(movementDto);
        if (validationTask == null) { return StatusCode(500); }
        var validationResult = await validationTask;
        if (validationResult == null) { return StatusCode(500); }
        if (!validationResult.IsValid) { return StatusCode(500); }
        
        return Ok(new PostMovementResponse(movementDto));
    }
}