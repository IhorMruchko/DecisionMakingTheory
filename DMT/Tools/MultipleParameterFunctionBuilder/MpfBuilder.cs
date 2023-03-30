namespace DMT.Tools.MultipleParameterFunctionBuilder;

public static class MpfBuilder
{
    public static IParameterReader MultipleFunctionAproximation(int x1Size, int x2Size, int x3Size, int ySize, int dataSize) 
        => new MultipleParameterFunction(x1Size, x2Size, x3Size, ySize, dataSize);
}
