using System.Collections.Generic;

namespace DMT.Tools.MultipleParameterFunctionBuilder;

public interface IAproximationDataProvider
{
    IResultDisplayer GetData(int dimension, out double mismatch, 
        out List<(int, double)> dataPoints, 
        out List<(int, double)> aproximationPoints);
}