using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using EmBackend.Models.Categories.Requests;
using EmBackend.Models.Categories.Responses;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController: ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly AuthRepository _authRepository;
    private readonly EntityMapper _entityMapper;
    private readonly Validation _validation;
    
    public CategoriesController(
        IRepository<Category> categoryRepository,
        IRepository<User> userRepository,
        AuthRepository authRepository,
        Validation validation,
        EntityMapper entityMapper
    )
    {
        _categoryRepository = categoryRepository;
        _authRepository = authRepository;
        _validation = validation;
        _entityMapper = entityMapper;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostCategoryResponse>> PostCategory(PostCategoryRequest data)
    {
        var category = _entityMapper.CategoryMapper.MapPostCategoryRequestToCategory(data);
        
        if (category == null) { return BadRequest(); }

        var categoryValidationResult = _validation.CategoryValidator.Validate(category);
        if (categoryValidationResult == null) { return StatusCode(500); }
        if (!categoryValidationResult.IsValid) { return BadRequest(categoryValidationResult.Errors); }
        
        var result = await _categoryRepository.Create(category);
        
        if (result == null) { return StatusCode(500); }
        
        var categoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(result);
        
        return Ok(new PostCategoryResponse(categoryDto));
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory(UpdateCategoryRequest data, string id)
    {
        var changesDocument = BsonUtilities.ToBsonDocument(data);
        
        var update = EntityOperationBuilder<Category>.BuildUpdateDefinition(changesDocument);
        var filter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
            builder.Eq(category => category.Id, id)
        );
        if (filter == null || update == null) { return BadRequest();}
        
        var category = await _categoryRepository.Update(update, filter);
        if (category == null) { return StatusCode(500); }
        
        var categoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(category);
        if (categoryDto == null) { return StatusCode(500); }

        return Ok(new UpdateCategoryResponse(categoryDto));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetCategoriesResponse>> GetCategories()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
            builder.Where(category =>
                category.Ownership == CategoryOwnership.Default ||
                (category.Ownership == CategoryOwnership.User && category.OwnerId == userId)
            )
        );
        if (filter == null) { return BadRequest();}

        var categories = await _categoryRepository.GetAll(filter);
        var categoriesDtos = categories
            .Select(category => _entityMapper.CategoryMapper.MapCategoryToCategoryDto(category))
            .ToList();
        
        
        return Ok(new GetCategoriesResponse(categoriesDtos));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        var filter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
            builder.Eq(category => category.Id, id)
        );
        if (filter == null) { return BadRequest(); }

        var deleteResult = await _categoryRepository.Delete(filter);
        if (deleteResult == null) { return BadRequest(); }
        
        return Ok("Category deleted");
    }
}