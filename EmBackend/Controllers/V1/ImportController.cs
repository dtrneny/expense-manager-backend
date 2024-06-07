using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using EmBackend.Models;
using EmBackend.Models.Helpers;
using EmBackend.Models.Imports.Movements.Requests;
using EmBackend.Models.Users.Requests;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<User> _userRepository;
    private readonly AuthRepository _authRepository;
    private readonly Validation _validation;

    public ImportController(
        IRepository<Movement> movementRepository,
        IRepository<Category> categoryRepository,
        IRepository<User> userRepository,
        AuthRepository authRepository,
        Validation validation
    )
    {
        _movementRepository = movementRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _authRepository = authRepository;
        _validation = validation;
    }

    [HttpPost]
    [Route("movements")]
    public async Task<ActionResult<MessageResponse>> ImportMovements(PostMovementImportRequest data)
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var userFilter = EntityOperationBuilder<User>.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Id, userId)
        );
        if (userFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var user = await _userRepository.GetOne(userFilter);
        if (user == null) { return NotFound("User could not be found."); }
        
        var categoryNames = data.ImportedMovements
            .SelectMany(movement => movement.CategoryNames)
            .Distinct()
            .ToList();
        
        var existenceFilter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
            builder.Where(category => 
                (category.OwnerId == userId && categoryNames.Contains(category.Name)) ||
                (category.Ownership == CategoryOwnership.Default && categoryNames.Contains(category.Name))
            )
        );
        if (existenceFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var categories = await _categoryRepository.GetAll(existenceFilter);
        var foundCategories = categories.ToArray();
        
        var foundCategoryNames = foundCategories.Select(category => category.Name);
        var notFoundCategoryNames = categoryNames.Except(foundCategoryNames);

        List<Category> validCategories = [];
        Dictionary<string, List<ValidationFailure>> categoryErrorsDict = [];
        
        foreach (var name in notFoundCategoryNames)
        {
            var categoryData = new Category
            {
                Name = name,
                OwnerId = userId,
                Ownership = CategoryOwnership.User
            };
            
            var categoryValidationResult = _validation.CategoryValidator.Validate(categoryData);
            if (categoryValidationResult == null)
            {
                categoryErrorsDict[name] = [];
                continue;
            }

            if (!categoryValidationResult.IsValid)
            {
                categoryErrorsDict[name] = categoryValidationResult.Errors;
                continue;
            }
            
            validCategories.Add(categoryData);
        }

        if (categoryErrorsDict.Count != 0) { return BadRequest(categoryErrorsDict); }

        List<Category> newCategories = [];
        foreach (var category in validCategories)
        {
            var newCategory = await _categoryRepository.Create(category);
            if (newCategory != null) { newCategories.Add(newCategory); }
        }
        
        var mergedCategories = foundCategories
            .ToList()
            .Concat(newCategories)
            .GroupBy(category => category.Id)
            .Select(group => group.First())
            .ToList();

        List<Movement> validMovements = [];
        Dictionary<string, List<ValidationFailure>> movementErrorsDict = [];
        
        foreach (var indexedMovementItem in data.ImportedMovements.Select((value, index) => new { value, index}))
        {
            var movement = indexedMovementItem.value;
            var index = indexedMovementItem.index;
            
            var categoryIds = mergedCategories
                .FindAll(category => movement.CategoryNames.Contains(category.Name))
                .Where(category => category.Id != null)
                .Select(category => category.Id!)
                .ToList();
            
            var movementData = new Movement {
                UserId = userId,
                Amount = movement.Amount,
                Label = movement.Label,
                Timestamp = movement.Timestamp,
                CategoryIds = categoryIds
            };
            
            var movementValidationResult = _validation.MovementValidator.Validate(movementData);
            if (movementValidationResult == null)
            {
                movementErrorsDict[index.ToString()] = [];
                continue;
            }

            if (!movementValidationResult.IsValid)
            {
                movementErrorsDict[index.ToString()] = movementValidationResult.Errors;
                continue;
            }
            
            validMovements.Add(movementData);
        }
        
        if (movementErrorsDict.Count != 0) { return BadRequest(movementErrorsDict); }
        
        List<Movement> newMovements = [];
        foreach (var movement in validMovements)
        {
            var newMovement = await _movementRepository.Create(movement);
            if (newMovement != null) { newMovements.Add(newMovement); }
        }

        var finalBalanceChange = newMovements.Sum(movement => movement.Amount);

        var updateRequest = new UpdateUserRequest(null, null, null, Balance: user.Balance + finalBalanceChange);
        var changesDocument = BsonUtility.ToBsonDocument(updateRequest);
        var update = EntityOperationBuilder<User>.BuildUpdateDefinition(changesDocument);
        if (update == null) { return BadRequest("The provided data could not be utilized for update."); }
        
        var userUpdate = await _userRepository.Update(update, userFilter);
        if (userUpdate == null) { return Problem("User balance could not be updated."); }
        
        return Ok("Movements were imported successfully.");
    }
}
