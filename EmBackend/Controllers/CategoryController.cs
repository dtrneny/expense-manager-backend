using EmBackend.Entities;
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
public class CategoryController: ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<User> _userRepository;
    private readonly EntityMapper _entityMapper;
    private readonly Validation _validation;
    
    public CategoryController(
        IRepository<Category> categoryRepository,
        IRepository<User> userRepository,
        Validation validation,
        EntityMapper entityMapper
    )
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _validation = validation;
        _entityMapper = entityMapper;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostCategoryResponse>> PostCategory(PostCategoryRequest data)
    {
        var category = _entityMapper.CategoryMapper.MapPostCategoryRequestToCategory(data);
        
        if (category == null) { return BadRequest(); }

        var validationResult = _validation.CategoryValidator.Validate(category);
        if (validationResult == null) { return StatusCode(500); }
        if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }
        
        var result = await _categoryRepository.Create(category);
        
        if (result == null) { return StatusCode(500); }
        
        var categoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(result);
        
        return Ok(new PostCategoryResponse(categoryDto));
    }
}