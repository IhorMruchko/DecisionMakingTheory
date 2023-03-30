using System;
using System.Linq;
using DMT.Tools;
using MathNet.Numerics.LinearAlgebra;

namespace DMT.Extensions;

public static class MatrixExtension
{
    public static Matrix<TElements> ToMatrix<TElements>(this TElements[,] source)
        where TElements : struct, IEquatable<TElements>, IFormattable 
        => Matrix<TElements>.Build.DenseOfArray(source);

    public static Matrix<TElements> ToRowMatrix<TElements>(this TElements[] source)
        where TElements : struct, IEquatable<TElements>, IFormattable 
        => Vector<TElements>.Build.DenseOfArray(source).ToRowMatrix();

    public static Matrix<TElements> ToColumnMatrix<TElements>(this TElements[] source)
        where TElements : struct, IEquatable<TElements>, IFormattable
        => Vector<TElements>.Build.DenseOfArray(source).ToColumnMatrix();

    public static Matrix<double> Normalize(this Matrix<double> matrix)
    {
        for (var i = 0; i < matrix.ColumnCount; ++i)
        {
            var min = matrix.Column(i).Min();
            var max = matrix.Column(i).Max();

            for (var j = 0; j < matrix.RowCount; ++j)
            {
                matrix[j, i] = (matrix[j, i] - min) / (max - min);
            }
        }
        return matrix;
    }
    
    public static double[] GetB(this double[,] source) 
        => source.ToMatrix().GetB();

    public static double[] GetB(this Matrix<double> source)
    {
        double[] res = new double[source.RowCount];

        for (int i = 0; i < source.RowCount; ++i)
            res[i] = (source.Row(i).Min() + source.Row(i).Max()) / 2;

        return res;
    }

    public static double[,] GetPsi(this double[,] lambda, double[,] xValues, int size) 
        => lambda.ToMatrix().GetPsi(xValues.ToMatrix(), size);

    public static double[,] GetPsi(this Matrix<double> lambda, Matrix<double> xValues, int size)
    {
        var psiValues = new double[size, xValues.ColumnCount];
        
        for(var i = 0; i < size; ++i)
            for (var j = 0; j < xValues.ColumnCount; ++j)
                psiValues[i, j] = lambda.Row(j).Select((value, p) => value * FunctionApproximation.EvaluatePolinomOffset(xValues[i, j], p)).Sum();
        
        return psiValues;
    }

    public static double[,] GetA(this double[,] psiValues, double[,] yValues) 
        => psiValues.ToMatrix().GetA(yValues.ToMatrix());

    public static double[,] GetA(this Matrix<double> psiValues, Matrix<double> yValues) 
        => psiValues.LeastSquare(yValues).Rotate();

    public static double[] GetFi(this double[,] aValues, double[,] psiValues, int index, int size) 
    {
        var result = new double[size];
        
        for (var i = 0; i < size; ++i)
            for (var j = 0; j < aValues.GetLength(1); ++j)
                result[i] += aValues[index, j] * psiValues[i, j];

        return result;
    }

    public static double[] GetC(this double[,] yValues, double[,] A1, double[,] A2, double[,] A3,
            double[,] Psi1, double[,] Psi2, double[,] Psi3, int size, int index)
    {
        var y = yValues.ToMatrix().Column(index).ToColumnMatrix();
        var matrix = Matrix<double>.Build.DenseOfColumnArrays(new double[][]
        {
           A1.GetFi(Psi1, index, size),
           A2.GetFi(Psi2, index, size),
           A3.GetFi(Psi3, index, size),
        });

        return matrix.LeastSquare(y).Column(0).ToArray();
    }

    public static double[,] Rotate(this Matrix<double> matrix)
    {
        var result = new double[matrix.ColumnCount, matrix.RowCount];
        
        for (var i = 0; i < matrix.ColumnCount; ++i)
            for (var j = 0; j < matrix.RowCount; ++j)
                result[i, j] = matrix[j, i];
        
        return result;
    }

    public static Matrix<double> LeastSquare(this Matrix<double> source, Matrix<double> vector)
       => (source.Transpose() * source).Inverse() * (source.Transpose() * vector);

    public static int InsertValues(this Matrix<double> source, double[,] dataSource, int index, int power, int offset)
    {
        for (var i = 0; i < dataSource.GetLength(1); ++i)
        {
            for (var p = 1; p < power + 1; ++p)
            {
                source[index, offset + i * power + p] = FunctionApproximation.EvaluatePolinomOffset(dataSource[index, i], p);
            }
        }
        return dataSource.GetLength(1) * power;
    }

    public static double[,] GetSubMatrix(this double[] source, int rows, int columns, int offset)
    {
        var result = new double[rows, columns];
        for (var i = 0; i < rows; ++i)
        {
            result[i, 0] = source[0];
            for (var j = 1; j < columns; ++j)
            {
                result[i, j] = source[offset + i * (columns - 1) + j];
            }
        }
        return result;
    }
}
