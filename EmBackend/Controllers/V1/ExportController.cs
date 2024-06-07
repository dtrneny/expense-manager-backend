using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using EmBackend.Models.Exports.Movements;
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
public class ExportController : ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly AuthRepository _authRepository;

    public ExportController(
        IRepository<Movement> movementRepository,
        IRepository<Category> categoryRepository,
        AuthRepository authRepository
    )
    {
        _movementRepository = movementRepository;
        _categoryRepository = categoryRepository;
        _authRepository = authRepository;
    }

    [HttpGet]
    [Route("movements")]
    public async Task<ActionResult> ExportMovements()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var movementFilter = MongoDbDefinitionBuilder.BuildFilterDefinition<Movement>(builder =>
            builder.Eq(movement => movement.UserId, userId)
        );
        var categoryFilter = MongoDbDefinitionBuilder.BuildFilterDefinition<Category>(builder =>
            builder.Where(movement => movement.Ownership == CategoryOwnership.Default || movement.OwnerId == userId)
        );
        if (movementFilter == null || categoryFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var movements = await _movementRepository.GetAll(movementFilter);
        var categories = await _categoryRepository.GetAll(categoryFilter);
        
        var categoryDictionary = categories.ToDictionary(category => category.Id!, category => category.Name);

        var exportMovementDtos = movements.Select(movement =>
        {
            var categoryNames = movement.CategoryIds
                .Select(id => categoryDictionary.GetValueOrDefault(id))
                .Where(name => name != null)
                .Select(name => name!)
                .ToList();

            return new ExportMovementDto(movement.Id!, movement.Amount, movement.Label, movement.Timestamp, categoryNames);
        });
        
        var fileBytes = FileUtility.GetFileBytesFromObject(new { exportedMovements = exportMovementDtos });
        
        return File(fileBytes, "application/json", "movements_export.json");
    }
}