using System;

namespace Variant11Avalonia;

public enum Page2Function
{
    Sinh,
    Square,
    Exp
}

public static class CalculationEngine
{
    public static double ComputePage1(double x, double y, double z)
    {
        var xPlusY = x + y;
        var d = Math.Abs(x - y);
        var s = Math.Sqrt(xPlusY);
        var p2 = Math.Pow(Math.Sin(z), 2);
        var num = d * (1 + p2 / s);
        var den = Math.Exp(d + x / 2.0);
        return y * Math.Pow(Math.Abs(x), 1.0 / 3.0) + Math.Pow(Math.Cos(y), 3) * (num / den);
    }

    public static double ComputePage2(double x, double y, Page2Function function, out string branch)
    {
        double fx = function switch
        {
            Page2Function.Sinh => Math.Sinh(x),
            Page2Function.Square => x * x,
            _ => Math.Exp(x)
        };

        var xy = x * y;
        var basePart = Math.Pow(fx + y, 2);

        if (xy > 0)
        {
            branch = "xy > 0";
            return basePart - Math.Sqrt(fx * y);
        }

        if (xy < 0)
        {
            branch = "xy < 0";
            return basePart + Math.Sqrt(Math.Abs(fx * y));
        }

        branch = "xy = 0";
        return basePart + 1;
    }

    public static double ComputePage3Y(double x, double a, double b)
    {
        return x + Math.Sqrt(Math.Abs(Math.Pow(x, 3) + a - b * Math.Exp(x)));
    }
}
