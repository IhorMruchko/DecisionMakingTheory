using DMT.Tools.MultipleParameterFunctionBuilder;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Windows;
using FilePath = System.IO.Path;

namespace FunctionAproximation.WPF;

public partial class MainWindow : Window
{
    private string? _sourceFilePath;
    private string? _targetFilePath;
    private IResultDisplayer? _resultDisplayer;
    
    public string? SourceFilePath
    {
        get => _sourceFilePath;
        set
        {
            _sourceFilePath = value;
            SourceFileTextBox.Text = FilePath.GetFileName(_sourceFilePath);
            if (TargetFilePath is not null)
                FindAproximation();
        }
    }

    public string? TargetFilePath
    {
        get => _targetFilePath;
        set
        {
            if (value?.Equals(_sourceFilePath, StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                ErrorMessages.TargetFilePathEqualsToSourceFilePath();
                return;
            }
            _targetFilePath = value;
            TargetFileTextBox.Text = FilePath.GetFileName(_targetFilePath);
            if (SourceFilePath is not null)
                FindAproximation();
        }
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SourceButton_Click(object sender, RoutedEventArgs e)
    {
        SourceFilePath = Dialogs.GetFilePath("Select source file path", Environment.CurrentDirectory);
    }

    private void TargetButton_Click(object sender, RoutedEventArgs e)
    {
        TargetFilePath = Dialogs.GetFilePath("Select target file path", Environment.CurrentDirectory);
    }

    private void YDimensionCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_resultDisplayer is null)
        {
            return;
        }

        _resultDisplayer.GetData(YDimensionCombobox.SelectedIndex, out var mismatch, out var dataPoints, out var aproximationPoints);
        var model = new PlotModel() { Title = $"Approximation for Y{YDimensionCombobox.SelectedIndex + 1}"};
        var dataPlot = new LineSeries
        {
            MarkerType = MarkerType.Circle, 
            MarkerSize = 4, 
            MarkerStroke = OxyColors.White, 
            MarkerFill = OxyColors.Blue, 
            StrokeThickness = 2,
            Color = OxyColors.Green 
        };
        var aproximationPlot = new LineSeries()
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 4,
            MarkerStroke = OxyColors.White,
            MarkerFill = OxyColors.Blue,
            StrokeThickness = 2,
            Color = OxyColors.Red
        };
        foreach(var (data, aproximation) in dataPoints.Zip(aproximationPoints))
        {
            dataPlot.Points.Add(new DataPoint(data.Item1, data.Item2));
            aproximationPlot.Points.Add(new DataPoint(aproximation.Item1, aproximation.Item2));
        }
        model.Series.Add(dataPlot);
        model.Series.Add(aproximationPlot);
        PlotSource.Model = model;
        MismatchTextBox.Text = mismatch.ToString();
    }


    private void FindAproximation()
    {
        UpdateCombobox();
        try
        {
            _resultDisplayer = MpfBuilder.MultipleFunctionAproximation(FirstXUpDown.Value!.Value,
                                                                       SecondXUpDown.Value!.Value,
                                                                       ThirdXUpDown.Value!.Value,
                                                                       YUpDown.Value!.Value,
                                                                       DataSizeUpDown.Value!.Value)
                .Read(SourceFilePath!)
                .Normalize()
                .EvaluateLambda(FirstXPoweUpDown.Value!.Value,
                                SecondXPoweUpDown.Value!.Value,
                                ThirdXPoweUpDown.Value!.Value)
                .EvaluateAMatrix()
                .EvaluateCMatrix()
                .Aproximate()
                .DisplayLambdas(out var lambdas)
                .DisplayAMatrix(out var aMatrix)
                .DisplayCMatrix(out var cMatrix)
                .DisplayAproximation(out var aproximationText);

            Results.Text += "Lambdas\n\n" + lambdas;
            Results.Text += "\n\nAMatrix\n\n" + aMatrix;
            Results.Text += "\n\nCMatrix\n\n" + cMatrix;
            Results.Text += "\n\nAproximation\n\n" + aproximationText;

        }
        catch (Exception ex)
        {
            ErrorMessages.OnError(ex.Message);
        }
    }

    private void UpdateCombobox()
    {
        if (YDimensionCombobox is null)
        {
            return;
        }
        YDimensionCombobox.ItemsSource = Enumerable.Range(1, YUpDown.Value!.Value).Select(i => $"Y{i}");
        YDimensionCombobox.SelectedIndex = 0;
    }
}
