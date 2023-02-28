using static System.Math;

namespace ParetoSet.Models;

public class TableModel
{
    private static readonly int rounding = 4;

    private double _xValue;
    private double _firstFunctionValue;
    private double _secondFunctionValue;
    
    public double XValue 
    { 
        get => Round(_xValue, rounding); 
        set => _xValue = value; 
    }

    public double FirstFunctionValue 
    { 
        get => Round(_firstFunctionValue, rounding); 
        set => _firstFunctionValue = value;
    }

    public double SecondFunctionValue 
    { 
        get => Round(_secondFunctionValue, rounding);
        set => _secondFunctionValue = value; 
    }

    public double Max => Round(Max(FirstFunctionValue, SecondFunctionValue), rounding);

    public double Min => Round(Min(FirstFunctionValue, SecondFunctionValue), rounding);

    public override string ToString()
    {
        return $"{XValue} - {FirstFunctionValue} - {SecondFunctionValue} - {Max} - {Min}";
    }
}
