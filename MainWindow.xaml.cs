using DMT.Extensions;
using ParetoSet.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ParetoSet.Views;
using OxyPlot.Series;
using OxyPlot;

namespace DecisionMakingTheory;
public partial class MainWindow : Window
{
    private readonly DataGridUserControl _dataGrid = new ();
    private readonly PlotUserControl _plot = new();

    private readonly Dictionary<string, Control> _views;

    public TableListModel RangeSource = new();

    public MainWindow()
    {
        _views = new()
        {
            ["Таблиця"] = _dataGrid,
            ["Графік"] = _plot
        };


        InitializeComponent();
        InitializeCombobox();
    }

    private void InitializeCombobox()
    {
        ViewChangerComboBox.ItemsSource = _views.Keys.ToArray();
        ViewChangerComboBox.SelectedIndex = 0;
        ResultsDisplayerContentControl.Content = _views["Таблиця"];
    }

    private void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        var start = LowerBoundInputTextBox.Text.Evaluate(exceptionHandler: HandleException);
        var end = UpperBoundInputTextBox.Text.Evaluate(exceptionHandler: HandleException);
        var step = Math.Pow(10, -StepInputSlider.Value);
        var firstFunctionBorder = FirstFunctionBorderInputTextBox.Text.Evaluate(exceptionHandler: HandleException);
        var secondFunctionBorder = SecondFunctionBorderInputTextBox.Text.Evaluate(exceptionHandler: HandleException);

        try
        {
            GenerateSequence(start, end, step, firstFunctionBorder, secondFunctionBorder);
            DisplayPlot(start, end, step,firstFunctionBorder, secondFunctionBorder);
            EvaluateItem();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void EvaluateItem()
    {
        var maxiMin = RangeSource.TableModels.Min(t => t.Max);
        var miniMax = RangeSource.TableModels.Max(t => t.Min);
        var maxiMinIndex = RangeSource.TableModels.FirstOrDefault(t => t.Min == miniMax)?.XValue;
        var miniMaxIndex = RangeSource.TableModels.FirstOrDefault(t => t.Max == maxiMin)?.XValue;
        MessageBox.Show($"Maximin = {maxiMin} ({maxiMinIndex});\nMinimax = {miniMax} ({miniMaxIndex})", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void DisplayPlot(double start, double end, double step, double firstFunctionBorder, double secondFunctionBorder)
    {
        _plot.Plot.GraphDisplayer.Series.Clear();
        _plot.Plot.GraphDisplayer.Series.Add(new FunctionSeries(x => FirstFunctionInputTextBox.Text.Evaluate(parameters:x), start, end, step, "f1(x)"));
        _plot.Plot.GraphDisplayer.Series.Add(new FunctionSeries(x => SecondFunctionInputTextBox.Text.Evaluate(parameters:x), start, end, step, "f2(x)"));
        _plot.Plot.GraphDisplayer.Series.Add(new LineSeries() { Title = "f1*"}.CreateLine(start, end, firstFunctionBorder));
        _plot.Plot.GraphDisplayer.Series.Add(new LineSeries() { Title = "f2*"}.CreateLine(start, end, secondFunctionBorder));
        _plot.Plot.GraphDisplayer.InvalidatePlot(true);
    }

    private void GenerateSequence(double start, double end, double step, double firstFunctionBorder, double secondFunctionBorder)
    {
        RangeSource.TableModels = (start, end, step).GetEnumerator().Select(current => new TableModel()
        {
            XValue = current,
            FirstFunctionValue = FirstFunctionInputTextBox.Text.Evaluate(parameters: current) / firstFunctionBorder,
            SecondFunctionValue = SecondFunctionInputTextBox.Text.Evaluate(parameters: current) / secondFunctionBorder
        }).ToList();

        _dataGrid.Source.ItemsSource = RangeSource.TableModels;
    }

    private void HandleException(Exception ex)
    {
        MessageBox.Show(ex.Message, "Bad formatting", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void ViewChangerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ResultsDisplayerContentControl.Content = _views[(string)((ComboBox)sender).SelectedValue];
    }
}
