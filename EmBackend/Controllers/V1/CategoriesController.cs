using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using EmBackend.Mappers;
using EmBackend.Models.Categories.Requests;
using EmBackend.Models.Categories.Responses;
using EmBackend.Models.Helpers;
using EmBackend.Repositories;
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
public class CategoriesController: ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly AuthRepository _authRepository;
    private readonly EntityMapper _entityMapper;
    private readonly ModelValidation _modelValidation;
    
    public CategoriesController(
        IRepository<Category> categoryRepository,
        AuthRepository authRepository,
        ModelValidation modelValidation,
        EntityMapper entityMapper
    )
    {
        _categoryRepository = categoryRepository;
        _authRepository = authRepository;
        _modelValidation = modelValidation;
        _entityMapper = entityMapper;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostCategoryResponse>> PostCategory(PostCategoryRequest data)
    {
        var categoryData = new Category
        {
            Name = data.Name,
            OwnerId = data.OwnerId,
            Ownership = data.Ownership
        };
        
        var categoryValidationResult = _modelValidation.CategoryValidator.Validate(categoryData);
        if (categoryValidationResult == null) { return StatusCode(500); }
        if (!categoryValidationResult.IsValid) { return BadRequest(categoryValidationResult.Errors); }
        
        var existenceFilter = MongoDbDefinitionBuilder.BuildFilterDefinition<Category>(builder =>
            builder.Where(category => category.OwnerId == data.OwnerId && category.Name == data.Name)
        );
        if (existenceFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var alreadyExistingCategory = await _categoryRepository.GetOne(existenceFilter);
        if (alreadyExistingCategory != null) { return BadRequest("Category with provided name already exists."); }
        
        var category = await _categoryRepository.Create(categoryData);
        if (category == null) { return Problem("Category could not be created."); }
        
        var categoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(category);
        if (categoryDto == null) { return StatusCode(500); }
        
        return Ok(new PostCategoryResponse(categoryDto));
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory(UpdateCategoryRequest data, string id)
    {
        var idValidationResult = _modelValidation.ObjectIdValidator.Validate(id);
        if (idValidationResult == null) { return StatusCode(500); }
        if (!idValidationResult.IsValid) { return BadRequest(idValidationResult.Errors); }
        
        var updateValidationResult = _modelValidation.UpdateCategoryValidator.Validate(data);
        if (updateValidationResult == null) { return StatusCode(500); }
        if (!updateValidationResult.IsValid) { return BadRequest(updateValidationResult.Errors); }
        
        var changesDocument = BsonUtility.ToBsonDocument(data);
        var update = MongoDbDefinitionBuilder.BuildUpdateDefinition<Category>(changesDocument);
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Category>(builder =>
            builder.Where(category => category.Id == id && category.Ownership != CategoryOwnership.Default)
        );
        if (filter == null || update == null) { return BadRequest("The provided data could not be utilized for filter or update."); }
        
        var category = await _categoryRepository.Update(update, filter);
        if (category == null) { return BadRequest("Category could not be updated."); }
        
        var categoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(category);
        if (categoryDto == null) { return StatusCode(500); }

        return Ok(new UpdateCategoryResponse(categoryDto));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetCategoriesResponse>> GetCategories()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Category>(builder =>
            builder.Where(category =>
                category.Ownership == CategoryOwnership.Default ||
                (category.Ownership == CategoryOwnership.User && category.OwnerId == userId)
            )
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var categories = await _categoryRepository.GetAll(filter);
        var categoriesDtos = categories
            .Select(category => _entityMapper.CategoryMapper.MapCategoryToCategoryDto(category))
            .ToList();
        
        return Ok(new GetCategoriesResponse(categoriesDtos));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageResponse>> DeleteCategory(string id)
    {
        var idValidationResult = _modelValidation.ObjectIdValidator.Validate(id);
        if (idValidationResult == null) { return StatusCode(500); }
        if (!idValidationResult.IsValid) { return BadRequest(idValidationResult.Errors); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<Category>(builder =>
            builder.Where(category => category.Id == id && category.Ownership != CategoryOwnership.Default)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var deleteResult = await _categoryRepository.Delete(filter);
        if (deleteResult == null) { return Problem("Category could not be deleted."); }
        if (deleteResult.DeletedCount == 0) { return BadRequest("Category could not be deleted."); }
        
        return Ok(new MessageResponse("Category was deleted."));
    }
}