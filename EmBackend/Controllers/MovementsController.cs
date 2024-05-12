using EmBackend.Entities;
using EmBackend.Models.Movements;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Movements.Responses;
using EmBackend.Repositories;
using EmBackend.Repositories.Auth;
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
    
    public MovementsController(IRepository<Movement> movementRepository, AuthRepository authRepository)
    {
        _movementRepository = movementRepository;
        _authRepository = authRepository;
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

        var movementDto = new MovementDto(result.Id, result.UserId, result.Amount, result.Label);
    
        return Ok(new PostMovementResponse(movementDto));
    }
}