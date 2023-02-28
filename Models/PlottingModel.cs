using OxyPlot;

namespace ParetoSet.Models;

public class PlottingModel
{
    public PlotModel GraphDisplayer { get; private set; } = new PlotModel() { Title = "Graphs", IsLegendVisible = true };
}
