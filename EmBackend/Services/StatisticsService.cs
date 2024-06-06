using EmBackend.Entities;
using EmBackend.Utilities.Enums;
using ScottPlot;

namespace EmBackend.Services;

public class StatisticsService
{
    public double GetMovementSum(IEnumerable<Movement> movements, MovementFilterType? type = MovementFilterType.All)
    {
        var sum = movements
            .ToList()
            .Where(movement =>
            {
                return type switch
                {
                    MovementFilterType.Income => movement.Amount > 0,
                    MovementFilterType.Expense => movement.Amount < 0,
                    MovementFilterType.All => true,
                    _ => false
                };
            })
            .Sum(movement => movement.Amount);

        return sum;
    }

    public Image? GetPiePlot(List<PieSlice> slices, bool showLegend, bool frameless, bool showLables)
    {
        Plot plot = new();
        
        var pie = plot.Add.Pie(slices);

        pie.ShowSliceLabels = showLables;
        plot.HideLegend();
        
        if (showLegend) { plot.ShowLegend(); }
        if (frameless) { plot.Layout.Frameless(); }

        return plot.GetImage(400, 300);
    }
    
    public Image? GetBarPlot(List<(Tick tick, int value)> bars)
    {
        Plot plot = new();

        List<Tick> ticks = [];
        
        foreach (var bar in bars)
        {
            plot.Add.Bar(position: bar.tick.Position, value: bar.value);
            ticks.Add(bar.tick);
        }

        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks.ToArray());
        plot.Axes.Bottom.MajorTickStyle.Length = 0;
        plot.HideGrid();

        plot.Axes.Margins(bottom: 0);

        return plot.GetImage(400, 300);
    }
    
}