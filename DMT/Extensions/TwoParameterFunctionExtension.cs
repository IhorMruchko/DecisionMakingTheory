using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.Extensions;

public static class TwoParameterFunctionExtension
{
    public static IEnumerable<IEnumerable<FunctionResult>> Result(this Func<double, double, double> function,
                                                                  DoubleRangeGenerator firstParameterRange,
                                                                  DoubleRangeGenerator secondParameterRange)
    {
        var result = new List<List<FunctionResult>>();
        foreach (var x1 in firstParameterRange)
        {
            var line = new List<FunctionResult>();
            foreach (var x2 in secondParameterRange)
            {
                line.Add(new FunctionResult()
                {
                    X1 = x1,
                    X2 = x2,
                    Result = function(x1, x2)
                });
            }
            result.Add(line);
        }

        return result;
    }

    public static FunctionResult? Constraint(this Func<double, double, double> function,
                                            DoubleRangeGenerator firstParameterRange,
                                            DoubleRangeGenerator secondParameterRange)
        => function.Result(firstParameterRange, secondParameterRange).Constraint();
}

public class FunctionResult : IComparable<FunctionResult>
{
    public double X1 { get; set; }

    public double X2 { get; set; }

    public double Result { get; set; }

    public int CompareTo(FunctionResult? other)
    {
        return other is null
            ? 1
            : Result.CompareTo(other.Result);
    }
}
