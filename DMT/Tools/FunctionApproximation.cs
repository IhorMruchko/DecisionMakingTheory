namespace DMT.Tools;

public static class FunctionApproximation
{
    public static double EvaluatePolinomOffset(double value, int p) 
        => p == 0 ? 0.5 : EvaluatePolinom((2 * value) - 1, p);
 
    private static double EvaluatePolinom(double value, int p) 
        => p == 0
        ? 1
        : p == 1
            ? value
            : 2 * value * EvaluatePolinom(value, p - 1) - EvaluatePolinom(value, p - 2);


}
