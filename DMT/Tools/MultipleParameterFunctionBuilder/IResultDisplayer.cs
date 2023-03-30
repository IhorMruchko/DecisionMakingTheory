namespace DMT.Tools.MultipleParameterFunctionBuilder;

public interface IResultDisplayer : IAproximationDataProvider
{
    IResultDisplayer DisplayLambdas(out string lambdas);

    IResultDisplayer DisplayAMatrix(out string aMatrix);

    IResultDisplayer DisplayCMatrix(out string cMatrix);
    
    IResultDisplayer DisplayAproximation(out string aproximation);
}