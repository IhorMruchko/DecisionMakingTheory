using System.Collections;
using System.Collections.Generic;
using static System.Math;

namespace DMT.Extensions;

public static class RangeExtension
{
    public static DoubleRangeGenerator GetEnumerator(this (double, double, double) source)
    {
        return new DoubleRangeGenerator(source);
    }

    public static DoubleRangeGenerator GetEnumerator(this (double, double) source)
    {
        return new DoubleRangeGenerator(source);
    }
}

public class DoubleRangeGenerator : IEnumerable<double>
{
    private readonly double _start;
    private readonly double _end;
    private readonly double _step = 0.1f;
    private double _current;

    public DoubleRangeGenerator((double start, double end, double step) source)
    {
        _start = source.start;
        _current = _start;
        _end = source.end + source.step;
        _step = source.step;
    }

    public DoubleRangeGenerator((double start, double end) source)
    {
        _start = source.start;
        _current = _start;
        _end = source.end;
    }

    public double Current => _current;

    public IEnumerator<double> GetEnumerator()
    {
        for (var i = _start; i <= _end; i += _step)
        {
            yield return Round(i, 4);
        }
    }

    public bool MoveNext()
    {
        _current += _step;
        return _current <= _end;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}