using Microsoft.VisualStudio.TestTools.UnitTesting;
using Variant11Avalonia;

namespace Variant11Avalonia.Tests;

/// <summary>
/// Набор модульных тестов для класса <see cref="CalculationEngine"/>.
/// </summary>
[TestClass]
public class CalculationEngineTests
{
    /// <summary>
    /// Демонстрирует работу базовых методов <see cref="Assert"/>.
    /// </summary>
    [TestMethod]
    public void TestMethod1()
    {
        Assert.AreEqual(4, 2 + 2);
        Assert.AreNotEqual(5, 2 + 2);
        Assert.IsTrue(10 > 1);
        Assert.IsFalse(1 > 10);
        Assert.IsNull(null);
        Assert.IsNotNull(new object());
    }

    /// <summary>
    /// Проверяет корректность вычисления функции страницы 1.
    /// </summary>
    [TestMethod]
    public void ComputePage1_WithValidArguments_ReturnsExpectedValue()
    {
        double result = CalculationEngine.ComputePage1(4.0, 2.0, 0.5);

        Assert.AreEqual(3.17191445871253, result, 1e-12);
    }

    /// <summary>
    /// Проверяет ветку <c>xy &gt; 0</c> для второй страницы.
    /// </summary>
    [TestMethod]
    public void ComputePage2_WhenProductIsPositive_UsesPositiveBranch()
    {
        double result = CalculationEngine.ComputePage2(1.0, 2.0, Page2Function.Square, out string branch);

        Assert.AreEqual("xy > 0", branch);
        Assert.AreEqual(7.585786437626905, result, 1e-12);
    }

    /// <summary>
    /// Проверяет ветку <c>xy &lt; 0</c> для второй страницы.
    /// </summary>
    [TestMethod]
    public void ComputePage2_WhenProductIsNegative_UsesNegativeBranch()
    {
        double result = CalculationEngine.ComputePage2(-1.0, 2.0, Page2Function.Sinh, out string branch);

        Assert.AreEqual("xy < 0", branch);
        Assert.AreEqual(2.213395281286458, result, 1e-12);
    }

    /// <summary>
    /// Проверяет ветку <c>xy = 0</c> для второй страницы.
    /// </summary>
    [TestMethod]
    public void ComputePage2_WhenProductIsZero_UsesZeroBranch()
    {
        double result = CalculationEngine.ComputePage2(0.0, 5.0, Page2Function.Exp, out string branch);

        Assert.AreEqual("xy = 0", branch);
        Assert.AreEqual(37.0, result, 1e-12);
    }

    /// <summary>
    /// Проверяет корректность вычисления функции страницы 3.
    /// </summary>
    [TestMethod]
    public void ComputePage3Y_WithValidArguments_ReturnsExpectedValue()
    {
        double result = CalculationEngine.ComputePage3Y(1.5, 2.0, 1.0);

        Assert.AreEqual(2.4451512734276646, result, 1e-12);
    }
}
