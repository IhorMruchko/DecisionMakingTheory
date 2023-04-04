using DMT.Extensions;
using DMT.Tools.MultipleParameterFunctionBuilder;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using static System.Math;

namespace DMT.Tools.MultipleFunctionRecreation;

public class MultipleParameterFunctionRecover : FunctionRecover
{
    internal MultipleParameterFunctionRecover(int x1Size, int x2Size, int x3Size, int ySize, int dataSize)
        : base(x1Size, x2Size, x3Size, ySize, dataSize) { }

    public override IFileReader SetParameters(params object[] parameters)
    {
        return this;
    }

    public override IAMatrixEvaluator EvaluateLambda(int firstXPower, int secondXPower, int thirdXPower)
    {
        var totalSize = _x1Values.GetLength(1) * firstXPower +
         +_x2Values.GetLength(1) * secondXPower + _x3Values.GetLength(1) * thirdXPower + 1;

        var lambdaMatrix = Matrix<double>.Build.Dense(_dataSize, totalSize);
        for (var i = 0; i < lambdaMatrix.RowCount; ++i)
        {
            lambdaMatrix[i, 0] = 0.5;
            var offset = 0;
            offset += lambdaMatrix.InsertValues(_x1Values, i, firstXPower, offset);
            offset += lambdaMatrix.InsertValues(_x2Values, i, secondXPower, offset);
            lambdaMatrix.InsertValues(_x3Values, i, thirdXPower, offset);
        }

        _lambda = lambdaMatrix.LeastSquare(_yValues.GetB().ToColumnMatrix()).ToColumnArrays()[0];
        return EvaluateLambdas(firstXPower, secondXPower, thirdXPower);
    }

    public override ICMatrixEvaluator EvaluateAMatrix()
    {
        _psi1 = _lambda1!.GetPsi(_x1Values, _dataSize);
        _psi2 = _lambda2!.GetPsi(_x2Values, _dataSize);
        _psi3 = _lambda3!.GetPsi(_x3Values, _dataSize);

        _aMatrix1 = _psi1.GetA(_yValues);
        _aMatrix2 = _psi2.GetA(_yValues);
        _aMatrix3 = _psi3.GetA(_yValues);

        return this;
    }

    public override IAproximationFinder EvaluateCMatrix()
    {
        for (var i = 0; i < _yValues.GetLength(1); ++i)
            _cMatrix[i] = _yValues.GetC(_aMatrix1!, _aMatrix2!, _aMatrix3!, _psi1!, _psi2!, _psi3!, _dataSize, i);

        return this;
    }

    public override IResultDisplayer Aproximate()
    {
        for (var i = 0; i < _dataSize; ++i)
        {
            for (var j = 0; j < _yValues.GetLength(1); ++j)
            {
                var sum = .0;
                sum += _cMatrix[j][0] * _aMatrix1!.GetFi(_psi1!, 0, _dataSize)[i];
                sum += _cMatrix[j][1] * _aMatrix2!.GetFi(_psi2!, 1, _dataSize)[i];
                sum += _cMatrix[j][2] * _aMatrix3!.GetFi(_psi3!, 2, _dataSize)[i];
                _aproximation[i, j] = sum;
            }
        }
        return this;
    }

    public override IResultDisplayer DisplayAproximation(out string aproximation)
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < _cMatrix.Length; ++i)
        {
            stringBuilder.Append($"\nФ({i + 1})(x1, x2, x3) = ");
            for (var j = 0; j < _cMatrix[i].Length; ++j)
            {
                var sign = _cMatrix[i][j] < 0 ? " - " : j == 0 ? " " : " + ";
                stringBuilder.Append($"{sign}{Abs(_cMatrix[i][j]):G6} * Ф{i + 1}{j + 1}(x{j + 1})");
            }

        }
        aproximation = stringBuilder.ToString();
        return this;
    }
}
