using System;
using Z.Expressions;
using static System.Math;

namespace DMT.Extensions;

public static class StringExtension
{
    public static int ToInt(this string value, int defaultValue=0)
    {
        return int.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    public static float ToFloat(this string value, float defaultValue = 0)
    {
        return float.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    public static double Evaluate(this string value, 
        int round=4, 
        Action<Exception>? exceptionHandler=null, 
        params object[] parameters)
    {
        try
        {
            return Round(Eval.Execute<double>(value, parameters), round);
        }
        catch(Exception ex) 
        {
            exceptionHandler?.Invoke(ex);
            return 0;
        }
    }

    public static double TryConvert(this string value, int currentRow, int currentColumn) 
        => double.TryParse(value, out double result)
        ? result
        : throw new ArgumentException($"Cannot convert [{value}] to a double at position ({currentRow + 1}, {currentColumn + 1})");
}
