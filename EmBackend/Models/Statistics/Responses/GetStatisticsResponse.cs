using EmBackend.Models.Categories;

namespace EmBackend.Models.Statistics.Responses;

public record GetStatisticsResponse(
    CategoryDto? KeyExpenseCategory,
    CategoryDto? KeyIncomeCategory,
    double OverallIncome,
    double OverallExpenses
);
