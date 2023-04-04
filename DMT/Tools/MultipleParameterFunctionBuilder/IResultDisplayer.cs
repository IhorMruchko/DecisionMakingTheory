namespace DMT.Tools.MultipleParameterFunctionBuilder;

public interface IResultDisplayer : IAproximationDataProvider
{
    IResultDisplayer GetLambdasString(out string lambdas);

    IResultDisplayer GetAMatrixesString(out string aMatrix);

    IResultDisplayer DisplayCMatrix(out string cMatrix);
    
    IResultDisplayer DisplayAproximation(out string aproximation);
}