using DMT.Extensions;
using DMT.Tools.MultipleParameterFunctionBuilder;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Math;

namespace DMT.Tools.MultipleFunctionRecreation;

public abstract class FunctionRecover : IParametterSetter,
                                        IFileReader,
                                        INormalizer,
                                        ILambdaEvaluator,
                                        IAMatrixEvaluator,
                                        ICMatrixEvaluator,
                                        IAproximationFinder,
                                        IResultDisplayer
{
    protected readonly int _dataSize;
    protected readonly int _totalLineSize;
    protected double[,] _x1Values;
    protected double[,] _x2Values;
    protected double[,] _x3Values;
    protected double[,] _yValues;

    protected double[]? _lambda;
    protected double[,]? _lambda1;
    protected double[,]? _lambda2;
    protected double[,]? _lambda3;

    protected double[,]? _psi1;
    protected double[,]? _psi2;
    protected double[,]? _psi3;

    protected double[,]? _aMatrix1;
    protected double[,]? _aMatrix2;
    protected double[,]? _aMatrix3;

    protected readonly double[][] _cMatrix;
    protected readonly double[,] _aproximation;

    internal FunctionRecover(int x1Size, int x2Size, int x3Size, int ySize, int dataSize)
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

    public abstract IFileReader SetParameters(params object[] parameters);

    public abstract IAMatrixEvaluator EvaluateLambda(int firstXPower, int secondXPower, int thirdXPower);
    
    public abstract ICMatrixEvaluator EvaluateAMatrix();

    public abstract IAproximationFinder EvaluateCMatrix();

    public abstract IResultDisplayer Aproximate();

    public abstract IResultDisplayer DisplayAproximation(out string aproximation);

    public virtual INormalizer Read(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        if (lines.Length < _dataSize)
            throw new ArgumentOutOfRangeException(nameof(lines.Length), "Length of the data in the file is less than expected value.");

        for (var i = 0; i < _dataSize; ++i)
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
    public virtual ILambdaEvaluator Normalize()
    {
        _x1Values = _x1Values.ToMatrix().Normalize().ToArray();
        _x2Values = _x2Values.ToMatrix().Normalize().ToArray();
        _x3Values = _x3Values.ToMatrix().Normalize().ToArray();
        _yValues = _yValues.ToMatrix().Normalize().ToArray();
        return this;
    }

    public virtual IResultDisplayer GetLambdasString(out string lambdas)
    {
        lambdas = _lambda1!.ToMatrix().ToMatrixString() + "\n"
           + _lambda2!.ToMatrix().ToMatrixString() + "\n"
           + _lambda3!.ToMatrix().ToMatrixString() + "\n";
        return this;
    }

    public virtual IResultDisplayer GetAMatrixesString(out string aMatrix)
    {

        aMatrix = _aMatrix1!.ToMatrix().ToMatrixString() + "\n"
            + _aMatrix2!.ToMatrix().ToMatrixString() + "\n"
            + _aMatrix3!.ToMatrix().ToMatrixString();
        return this;
    }

    public virtual IResultDisplayer DisplayCMatrix(out string cMatrix)
    {
        cMatrix = Matrix<double>.Build.DenseOfRowArrays(_cMatrix).ToMatrixString();
        return this;
    }
    
    public virtual IResultDisplayer GetData(int dimension, out double mismatch, out List<(int, double)> dataPoints, out List<(int, double)> aproximationPoints)
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

    protected IAMatrixEvaluator EvaluateLambdas(int firstXPower, int secondXPower, int thirdXPower)
    {
        _lambda1 = _lambda!.GetSubMatrix(_x1Values.GetLength(1), firstXPower + 1, 0);
        _lambda2 = _lambda!.GetSubMatrix(_x2Values.GetLength(1), secondXPower + 1, _x1Values.GetLength(1) * firstXPower);
        _lambda3 = _lambda!.GetSubMatrix(_x3Values.GetLength(1), thirdXPower + 1, _x1Values.GetLength(1) * firstXPower + _x2Values.GetLength(1) * secondXPower);
        return this;
    }
}
