using OxyPlot;
using ParetoSet.Models;
using System.Windows.Controls;

namespace ParetoSet.Views;

public partial class PlotUserControl : UserControl
{
    public PlotUserControl()
    {
        InitializeComponent();
    }

    public PlottingModel Plot => (PlottingModel)DataContext;
}
