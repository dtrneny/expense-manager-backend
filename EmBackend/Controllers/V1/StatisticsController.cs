using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using EmBackend.Models.Categories;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Utilities;
using EmBackend.Utilities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScottPlot;

namespace EmBackend.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IRepository<Movement> _movementRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly AuthRepository _authRepository;
    private readonly StatisticsService _statisticsService;

    public StatisticsController(
        IRepository<Movement> movementRepository,
        IRepository<Category> categoryRepository,
        AuthRepository authRepository
    )
    {
        _movementRepository = movementRepository;
        _categoryRepository = categoryRepository;
        _authRepository = authRepository;
        _statisticsService = new StatisticsService();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetStatistics()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = EntityOperationBuilder<Movement>.BuildFilterDefinition(builder =>
            builder.Eq(movement => movement.UserId, userId)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var movements = await _movementRepository.GetAll(filter);
        
        var movementList = movements.ToList();
        
        var categoryFilter = EntityOperationBuilder<Category>.BuildFilterDefinition(builder =>
            builder.Where(movement => movement.Ownership == CategoryOwnership.Default || movement.OwnerId == userId)
        );
        if (categoryFilter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var categories = await _categoryRepository.GetAll(categoryFilter);
        
        var categoryCounts = movementList
            .SelectMany(movement => movement.CategoryIds)
            .GroupBy(id => id)
            .Select(group => new { CategoryId = group.Key, Count = group.Count() })
            .ToList();
        
        var categoryCountWithNames = categoryCounts
            .Join(
                categories,
                count => count.CategoryId,
                category => category.Id,
                (count, category) => new { CategoryName = category.Name, count.Count }
            )
            .ToList();
        
        var overallExpenses = _statisticsService.GetMovementSum(movementList, MovementFilterType.Expense);
        var overallIncome = _statisticsService.GetMovementSum(movementList, MovementFilterType.Income);

        List<PieSlice> expensesIncomeSlices =
        [
            new PieSlice { Value = -overallExpenses, FillColor = Colors.Red, Label = $"Expenses: { overallExpenses }" },
            new PieSlice { Value = overallIncome, FillColor = Colors.Blue, Label = $"Income: { overallIncome }" }
        ];

        var piePlotImage = _statisticsService.GetPiePlot(expensesIncomeSlices, false, true, true);
        
        var bars = categoryCountWithNames
            .Select((value, index) => new { value, index })
            .Select(obj => (new Tick(obj.index, obj.value.CategoryName), obj.value.Count))
            .ToList();
        
        var barPlotImage = _statisticsService.GetBarPlot(bars);

        if (barPlotImage == null || piePlotImage == null) { return BadRequest(); }
        
        var barPlotBytes = barPlotImage.GetImageBytes();
        var piePlotBytes = piePlotImage.GetImageBytes();
        
        var zipDict = new Dictionary<string, byte[]>
        {
            { "category_occurence_plot.jpeg", barPlotBytes },
            { "expenses_x_income_plot.jpeg", piePlotBytes }
        };

        var zip = CompressionUtility.CreateZipFromByteArrays(zipDict);
        
        return File(zip, "application/zip", "plots.zip");
    }
}