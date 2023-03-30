using DMT.Extensions;
using static System.Math;

//double F12(double x1, double x2)
//{
//    return -8 * x1 * Pow(x2, 2) + 9 * Pow(x2, 2) - 37 * x2 + 4 * Pow(x1, 2) + 50;
//}

//double F21(double x1, double x2)
//{
//    return -2 * Pow(x2 - 5, 2) - 9 * Pow(x1 - 0.95, 2) * x2 + 2 * x1 * x2 + 50;
//}

//double F12(double x1, double x2)
//{
//    return Pow(Cos(3 * Sqrt(Pow(x1, 2) + Pow(x2, 2))), 2);
//}

//double F21(double x1, double x2)
//{
//    return Sin(Sqrt(Pow(x1 - 15, 2) + Pow(x2 - 15, 2)));
//}

//var f12 = F12;
//var f21 = F21;
//var range = new DoubleRangeGenerator((0, 2, 0.01));
//var result = f12.Result(range, range);
//var result2 = f21.Result(range, range);

//var f12Star = result.Constraint();
//var f21Star = result2.Constraint();

//result.Save("f12.txt");
//result2.Save("f21.txt");

var item1 = new double[,]
{
    {1, 2, 3 ,4 },
    {4, 3, 2, 1},
    {-2, -1, -1, -3},
};

var item2 = new double[,]
{
    {1, 2, 3 ,4 },
    {4, 3, 2, 1},
    {-2, -1, -1, -3},
};

var result1 = GetB(item1);
var rot = item2.ToMatrix().Rotate().ToMatrix();
Console.WriteLine(rot.ToMatrixString());
var result2 = item2.GetB();

Console.WriteLine(string.Join("; ", result1));
Console.WriteLine();
Console.WriteLine(string.Join("; ", result2));

static double[,] Normalization(double[,] matrix)
{
    int size1 = matrix.GetLength(0);
    int size2 = matrix.GetLength(1);
    Console.WriteLine($"Size1 = {size1}, size2 = {size2}");
    for (int i = 0; i < size2; ++i)
    {
        double min = matrix[0, i];
        double max = matrix[0, i];
        for (int j = 0; j < size1; ++j)
        {
            if (matrix[j, i] > max)
                max = matrix[j, i];
            if (matrix[j, i] < min)
                min = matrix[j, i];
        }
        for (int j = 0; j < size1; ++j)
        {
            matrix[j, i] = (matrix[j, i] - min) / (max - min);
        }
    }

    return matrix;
}

static double[] GetB(double[,] y)
{
    int size1 = y.GetLength(0);
    int size2 = y.GetLength(1);

    double[] res = new double[size1];
    for (int i = 0; i < size1; ++i)
    {
        double min = y[i, 0];
        double max = y[i, 0];
        for (int j = 0; j < size2; ++j)
        {
            if (y[i, j] > max)
                max = y[i, j];
            if (y[i, j] < min)
                min = y[i, j];
        }
        res[i] = (max + min) / 2;
    }
    return res;
}
