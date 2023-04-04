using DMT.Tools.MultipleFunctionRecreation;

namespace DMT.Tools.MultipleParameterFunctionBuilder;

public static class MpfBuilder
{
    public static IParametterSetter MultipleFunctionAproximation(int x1Size, int x2Size, int x3Size, int ySize, int dataSize) 
        => new MultipleParameterFunctionRecover(x1Size, x2Size, x3Size, ySize, dataSize);

    public static IParametterSetter MultipleConnectedFunctionAproximation(int x1Size, int x2Size, int x3Size, int ySize, int dataSize)
        => new MultipleConnectedParameterFunctionRecover(x1Size, x2Size, x3Size, ySize, dataSize);
}
