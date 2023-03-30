using DMT.Extensions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Math;

namespace DMT.Tools.MultipleParameterFunctionBuilder;

public class MultipleParameterFunction : IParameterReader,
                                         INormalizer,
                                         ILambdaEvaluator,
                                         IAMatrixEvaluator,
                                         ICMatrixEvaluator,
                                         IAproximationFinder,
                                         IResultDisplayer
{
    private int _dataSize;
    private int _totalLineSize;
    private double[,] _x1Values;
    private double[,] _x2Values;
    private double[,] _x3Values;
    private double[,] _yValues;

    private double[]? _lambda;
    private double[,]? _lambda1;
    private double[,]? _lambda2;
    private double[,]? _lambda3;

    private double[,]? _psi1;
    private double[,]? _psi2;
    private double[,]? _psi3;

    private double[,]? _aMatrix1;
    private double[,]? _aMatrix2;
    private double[,]? _aMatrix3;

    private double[][] _cMatrix;
    private double[,] _aproximation;

    internal MultipleParameterFunction(int x1Size, int x2Size, int x3Size, int ySize, int dataSize)
    {
        _dataSize = dataSize;
        _totalLineSize = x1Size + x2Size + x3Size + ySize;
        _x1Values = new double[dataSize, x1Size];
        _x2Values = new double[dataSize, x2Size];
        _x3Values = new double[dataSize, x3Size];
        _yValues = new double[dataSize, ySize];
        _cMatrix = new double[ySize][];
        _aproximation = new double[dataSize, ySize];
    }

    public INormalizer Read(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        if (lines.Length < _dataSize)
            throw new ArgumentOutOfRangeException(nameof(lines.Length), "Length of the data in the file is less than expected value.");

        for (var i = 0; i < lines.Length; ++i)
        {
            var line = lines[i].Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (line.Length < _totalLineSize)
                throw new ArgumentOutOfRangeException(nameof(line.Length), "Length of the data row in the file is less than expected value.");

            var currentOffset = 0;
            currentOffset += _x1Values.InsertConvetedValues(line, i, currentOffset);
            currentOffset += _x2Values.InsertConvetedValues(line, i, currentOffset);
            currentOffset += _x3Values.InsertConvetedValues(line, i, currentOffset);
            _yValues.InsertConvetedValues(line, i, currentOffset);
        }
        return this;
    }

    public ILambdaEvaluator Normalize()
    {
        _x1Values = _x1Values.ToMatrix().Normalize().ToArray();
        _x2Values = _x2Values.ToMatrix().Normalize().ToArray();
        _x3Values = _x3Values.ToMatrix().Normalize().ToArray();
        _yValues = _yValues.ToMatrix().Normalize().ToArray();
        return this;
    }

    public IAMatrixEvaluator EvaluateLambda(int firstXPower, int secondXPower, int thirdXPower)
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

    private IAMatrixEvaluator EvaluateLambdas(int firstXPower, int secondXPower, int thirdXPower)
    {
        _lambda1 = _lambda!.GetSubMatrix(_x1Values.GetLength(1), firstXPower + 1, 0);
        _lambda2 = _lambda!.GetSubMatrix(_x2Values.GetLength(1), secondXPower + 1, _x1Values.GetLength(1) * firstXPower);
        _lambda3 = _lambda!.GetSubMatrix(_x3Values.GetLength(1), thirdXPower + 1, _x1Values.GetLength(1) * firstXPower + _x2Values.GetLength(1) * secondXPower);
        return this;
    }

    public ICMatrixEvaluator EvaluateAMatrix()
    {
        _psi1 = _lambda1!.GetPsi(_x1Values, _dataSize);
        _psi2 = _lambda2!.GetPsi(_x2Values, _dataSize);
        _psi3 = _lambda3!.GetPsi(_x3Values, _dataSize);

        _aMatrix1 = _psi1.GetA(_yValues);
        _aMatrix2 = _psi2.GetA(_yValues);
        _aMatrix3 = _psi3.GetA(_yValues);

        return this;
    }

    public IAproximationFinder EvaluateCMatrix()
    {
        for (var i = 0; i < _yValues.GetLength(1); ++i)
            _cMatrix[i] = _yValues.GetC(_aMatrix1!, _aMatrix2!, _aMatrix3!, _psi1!, _psi2!, _psi3!, _dataSize, i);

        return this;
    }

    public IResultDisplayer Aproximate()
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

    public IResultDisplayer DisplayLambdas(out string lambdas)
    {
        lambdas = string.Join("\t\t", _lambda!);
        return this;
    }

    public IResultDisplayer DisplayAMatrix(out string aMatrix)
    {
        aMatrix = _aMatrix1!.ToMatrix().ToMatrixString() + "\n"
            + _aMatrix2!.ToMatrix().ToMatrixString() + "\n"
            + _aMatrix3!.ToMatrix().ToMatrixString();
        return this;
    }

    public IResultDisplayer DisplayCMatrix(out string cMatrix)
    {
        cMatrix = Matrix<double>.Build.DenseOfRowArrays(_cMatrix).ToMatrixString();
        return this;
    }

    public IResultDisplayer DisplayAproximation(out string aproximation)
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

    public IResultDisplayer GetData(int dimension, out double mismatch, out List<(int, double)> dataPoints, out List<(int, double)> aproximationPoints)
    {
        dataPoints = new List<(int, double)>(_dataSize);
        aproximationPoints = new List<(int, double)>(_dataSize);
        mismatch = 0;
        for (var i = 0; i < _dataSize; ++i)
        {
            dataPoints.Add((i, _yValues[i, dimension]));
            aproximationPoints.Add((i, _aproximation[i, dimension]));
            if (Abs(_yValues[i, dimension] - _aproximation[i, dimension]) > mismatch)
            {
                mismatch = Abs(_yValues[i, dimension] - _aproximation[i, dimension]);
            }
        }

        return this;
    }
}
