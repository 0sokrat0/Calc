using System;

namespace Variant11Avalonia;

/// <summary>
/// Перечисление доступных вариантов функции <c>f(x)</c> для второй страницы.
/// </summary>
public enum Page2Function
{
    /// <summary>
    /// Гиперболический синус.
    /// </summary>
    Sinh,

    /// <summary>
    /// Квадрат числа.
    /// </summary>
    Square,

    /// <summary>
    /// Экспонента.
    /// </summary>
    Exp
}

/// <summary>
/// Выполняет вычисления математических функций, используемых в приложении.
/// </summary>
public static class CalculationEngine
{
    /// <summary>
    /// Вычисляет значение функции <c>b</c> для первой страницы.
    /// </summary>
    /// <param name="x">Значение <c>x</c>.</param>
    /// <param name="y">Значение <c>y</c>.</param>
    /// <param name="z">Значение <c>z</c>.</param>
    /// <returns>Результат вычисления функции <c>b</c>.</returns>
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

    /// <summary>
    /// Вычисляет значение функции <c>a</c> для второй страницы и сообщает использованную ветку условия.
    /// </summary>
    /// <param name="x">Значение <c>x</c>.</param>
    /// <param name="y">Значение <c>y</c>.</param>
    /// <param name="function">Выбранный вариант функции <c>f(x)</c>.</param>
    /// <param name="branch">Текстовое описание использованной ветки условия.</param>
    /// <returns>Результат вычисления функции <c>a</c>.</returns>
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

    /// <summary>
    /// Вычисляет значение функции <c>y</c> для третьей страницы.
    /// </summary>
    /// <param name="x">Текущее значение <c>x</c>.</param>
    /// <param name="a">Параметр <c>a</c>.</param>
    /// <param name="b">Параметр <c>b</c>.</param>
    /// <returns>Результат вычисления функции <c>y</c>.</returns>
    public static double ComputePage3Y(double x, double a, double b)
    {
        return x + Math.Sqrt(Math.Abs(Math.Pow(x, 3) + a - b * Math.Exp(x)));
    }
}
