using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DMT.Extensions;

public static class EnumerableExtension
{
    public static FunctionResult? Constraint(this IEnumerable<IEnumerable<FunctionResult>> source)
        => source.Select(innerLine => innerLine.Min()).Max();

    public static void Save(this IEnumerable<IEnumerable<FunctionResult>> source, string filePath)
    {
        var result = new StringBuilder();
        foreach (var x1Line in source)
        {
            result.AppendLine($"{x1Line.First().X1}:");
            foreach (var x2Line in x1Line)
            {
                result.AppendLine($"\t {x2Line.X2} = {x2Line.Result}");
            }
        }

        var constraint = source.Constraint();
        if (constraint is not null)
            result.AppendLine($"maxmin func = ({constraint.X1}, {constraint.X2}) = {constraint.Result}");

        File.WriteAllText(filePath, result.ToString());
    }

    public static int InsertConvetedValues(this double[,] source, string[] dataSource, int currentRow, int currentOffset)
    {
        for (var i = 0; i < source.GetLength(1); ++i)
            source[currentRow, i] = dataSource[currentOffset + i].TryConvert(currentRow, i);
        
        return source.GetLength(1);
    }
}
