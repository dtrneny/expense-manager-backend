using EmBackend.Entities;
using EmBackend.Payloads.Movements;
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
    public async Task<ActionResult<CreateResponse>> CreateMovement(CreateRequest data)
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);

        if (userId == null) { return Unauthorized(); }
        
        var result = await _movementRepository.Create(new Movement()
        {
            Id = null,
            UserId = userId,
            Amount = data.Amount,
            Label = data.Label
        });
    
        if (result == null) { return StatusCode(500); }
    
        return Ok(result);
    }
}