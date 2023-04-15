using System;
using System.Linq;
using System.Text;
using DMT.Extensions;
using DMT.Tools.MultipleParameterFunctionBuilder;
using MathNet.Numerics.LinearAlgebra;

namespace DMT.Tools.MultipleFunctionRecreation;

public class MultipleConnectedParameterFunctionRecover : FunctionRecover
{
    private double _precision;

    public MultipleConnectedParameterFunctionRecover(int x1Size, int x2Size, int x3Size, int ySize, int dataSize)
        : base(x1Size, x2Size, x3Size, ySize, dataSize) { }

    public override IFileReader SetParameters(params object[] parameters)
    {
        if (parameters.Length != 1 || parameters[0] is not int value || value < 2 || value > 20)
        {
            throw new ArgumentException("Value must be one object array with integer between 2 and 20.", nameof(parameters));
        }
        _precision = Math.Pow(10, -value);
        return this;
    }

    public override IAMatrixEvaluator EvaluateLambda(int firstXPower, int secondXPower, int thirdXPower)
    {
        var totalSize = _x1Values.GetLength(1) * firstXPower +
        +_x2Values.GetLength(1) * secondXPower + _x3Values.GetLength(1) * thirdXPower + 1;

        var lambdaMatrix = Matrix<double>.Build.Dense(_dataSize, totalSize);
        for (var i = 0; i < lambdaMatrix.RowCount; ++i)
        {
            lambdaMatrix[i, 0] = Math.Log(1 + 0.5);
            var offset = 0;
            offset += lambdaMatrix.InsertValues(_x1Values, i, firstXPower, offset, _precision);
            offset += lambdaMatrix.InsertValues(_x2Values, i, secondXPower, offset, _precision);
            lambdaMatrix.InsertValues(_x3Values, i, thirdXPower, offset, _precision);
        }

        _lambda = lambdaMatrix.LeastSquare(_yValues.GetB().Select(value => Math.Log(1 + _precision + value)).ToArray()
            .ToColumnMatrix()).ToColumnArrays()[0];
        return EvaluateLambdas(firstXPower, secondXPower, thirdXPower);
    }

    public override ICMatrixEvaluator EvaluateAMatrix()
    {
        _psi1 = _lambda1!.GetPsi(_x1Values, _dataSize, _precision);
        _psi2 = _lambda2!.GetPsi(_x2Values, _dataSize, _precision);
        _psi3 = _lambda3!.GetPsi(_x3Values, _dataSize, _precision);

        _aMatrix1 = _psi1.GetA(_yValues, _precision);
        _aMatrix2 = _psi2.GetA(_yValues, _precision);
        _aMatrix3 = _psi3.GetA(_yValues, _precision);
        return this;
    }

    public override IAproximationFinder EvaluateCMatrix()
    {
        for (var i = 0; i < _yValues.GetLength(1); ++i)
            _cMatrix[i] = _yValues.GetC(_aMatrix1!, _aMatrix2!, _aMatrix3!, _psi1!, _psi2!, _psi3!, _dataSize, i, _precision);

        return this;
    }

    public override IResultDisplayer Aproximate()
    {
        for (var i = 0; i < _dataSize; ++i)
        {
            for (var j = 0; j < _yValues.GetLength(1); ++j)
            {
                var sum = .0;
                sum += _cMatrix[j][0] * Math.Log(_aMatrix1!.GetFiConnected(_psi1!, 0, _dataSize)[i] + 1);
                sum += _cMatrix[j][1] * Math.Log(_aMatrix2!.GetFiConnected(_psi2!, 1, _dataSize)[i] + 1);
                sum += _cMatrix[j][2] * Math.Log(_aMatrix3!.GetFiConnected(_psi3!, 2, _dataSize)[i] + 1);
                _aproximation[i, j] = Math.Exp(sum) - 1;
            }
        }
        return this;
    }

    public override IResultDisplayer DisplayAproximation(out string aproximation)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < _cMatrix.Length; ++i)
        {
            builder.Append($"\nФ({i + 1})(x1, x2, x3) = exp ").Append('{');
            for (var j = 0; j < _cMatrix[i].Length; ++j)
            {
                var sign = _cMatrix[i][j] < 0 ? " - " : j == 0 ? " " : " + ";
                builder.Append($"{sign}{Math.Abs(_cMatrix[i][j]):G6} * ln(1+ Ф{i + 1}{j + 1}(x{j + 1}))");
            }
            builder.Append('}');

        }

        aproximation = builder.ToString();
        return this;
    }
}
