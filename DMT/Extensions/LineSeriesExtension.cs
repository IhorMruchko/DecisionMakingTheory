using OxyPlot;
using OxyPlot.Series;

namespace DMT.Extensions;

public static class LineSeriesExtension
{
    public static LineSeries CreateLine(this LineSeries source, double start, double end, double yValue)
    {
        source.Points.Add(new DataPoint(start, yValue));
        source.Points.Add(new DataPoint(end, yValue));
        return source;
    }
}
