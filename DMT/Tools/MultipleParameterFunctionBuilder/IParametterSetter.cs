namespace DMT.Tools.MultipleParameterFunctionBuilder;

public interface IParametterSetter
{
    IFileReader SetParameters(params object[] parameters);
}
