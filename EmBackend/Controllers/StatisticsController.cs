using EmBackend.Entities;
using EmBackend.Models.Categories;
using EmBackend.Models.Statistics.Responses;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Utilities;
using EmBackend.Utilities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly AuthRepository _authRepository;
    private readonly EntityMapper _entityMapper;
    private StatisticsService _statisticsService;

    public StatisticsController(
        IRepository<Movement> movementRepository,
        IRepository<Category> categoryRepository,
        AuthRepository authRepository,
        EntityMapper entityMapper
    )
    {
        _movementRepository = movementRepository;
        _categoryRepository = categoryRepository;
        _authRepository = authRepository;
        _entityMapper = entityMapper;
        _statisticsService = new StatisticsService();
    }
    
    [HttpGet]
    public async Task<ActionResult<GetStatisticsResponse>> GetStatistics()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = EntityOperationBuilder<Movement>.BuildFilterDefinition(builder =>
            builder.Eq(movement => movement.UserId, userId)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var movements = await _movementRepository.GetAll(filter);
        
        var movementList = movements.ToList();
        
        var keyExpenseCategoryId = _statisticsService.GetKeyMovementCategoryId(movementList, MovementFilterType.Expense);
        var keyIncomeCategoryId = _statisticsService.GetKeyMovementCategoryId(movementList, MovementFilterType.Income);

        CategoryDto? keyExpenseCategoryDto = null;

        if (keyExpenseCategoryId != null)
        {
            var keyExpenseFilter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
                builder.Eq(category => category.Id, keyExpenseCategoryId)
            );
            if (keyExpenseFilter != null)
            {
                var keyExpenseCategory = await _categoryRepository.GetOne(keyExpenseFilter);
                if (keyExpenseCategory != null)
                {
                    keyExpenseCategoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(keyExpenseCategory);
                }
            }
        }
        
        CategoryDto? keyIncomeCategoryDto = null;

        if (keyIncomeCategoryId != null)
        {
            var keyIncomeFilter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
                builder.Eq(category => category.Id, keyExpenseCategoryId)
            );
            if (keyIncomeFilter != null)
            {
                var keyIncomeCategory = await _categoryRepository.GetOne(keyIncomeFilter);
                if (keyIncomeCategory != null)
                {
                    keyIncomeCategoryDto = _entityMapper.CategoryMapper.MapCategoryToCategoryDto(keyIncomeCategory);
                }
            }
        }
        
        var overallExpenses = _statisticsService.GetMovementSum(movementList, MovementFilterType.Expense);
        var overallIncome = _statisticsService.GetMovementSum(movementList, MovementFilterType.Income);

        return Ok(new GetStatisticsResponse(
            keyExpenseCategoryDto, 
            keyIncomeCategoryDto,
            overallIncome,
            overallExpenses
        ));
    }
}