namespace DMT.Tools.MultipleParameterFunctionBuilder;

public interface ILambdaEvaluator
{    
    IAMatrixEvaluator EvaluateLambda(int firstXPower, int secondXPower, int thirdXPower);
}