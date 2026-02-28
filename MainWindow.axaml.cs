using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Variant11Avalonia;

public partial class MainWindow : Window
{
    private bool _exitConfirmed;

    public MainWindow()
    {
        InitializeComponent();
        BuildEmptyChart();
        Closing += OnClosingWithConfirmation;
    }

    private static bool TryParse(string? input, out double value)
    {
        return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
               || double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
    }

    private void CalculatePage1Click(object? sender, RoutedEventArgs e)
    {
        Page1ValidationText.Text = string.Empty;

        if (!TryParse(Page1XTextBox.Text, out var x) ||
            !TryParse(Page1YTextBox.Text, out var y) ||
            !TryParse(Page1ZTextBox.Text, out var z))
        {
            Page1ValidationText.Text = "Введите корректные числовые значения x, y, z.";
            Page1ResultTextBox.Text = string.Empty;
            return;
        }

        var xPlusY = x + y;
        if (xPlusY <= 0)
        {
            Page1ValidationText.Text = "Для sqrt(x + y) требуется x + y > 0.";
            Page1ResultTextBox.Text = string.Empty;
            return;
        }

        var b = CalculationEngine.ComputePage1(x, y, z);
        Page1ResultTextBox.Text = b.ToString("G17", CultureInfo.InvariantCulture);
    }

    private void ClearPage1Click(object? sender, RoutedEventArgs e)
    {
        Page1XTextBox.Text = string.Empty;
        Page1YTextBox.Text = string.Empty;
        Page1ZTextBox.Text = string.Empty;
        Page1ResultTextBox.Text = string.Empty;
        Page1ValidationText.Text = string.Empty;
    }

    private void CalculatePage2Click(object? sender, RoutedEventArgs e)
    {
        Page2ValidationText.Text = string.Empty;
        Page2ConditionText.Text = string.Empty;

        if (!TryParse(Page2XTextBox.Text, out var x) || !TryParse(Page2YTextBox.Text, out var y))
        {
            Page2ValidationText.Text = "Введите корректные числовые значения x и y.";
            Page2ResultTextBox.Text = string.Empty;
            return;
        }

        double fx;
        if (FxSinhRadio.IsChecked == true)
        {
            fx = Math.Sinh(x);
        }
        else if (FxSquareRadio.IsChecked == true)
        {
            fx = x * x;
        }
        else
        {
            fx = Math.Exp(x);
        }

        var xy = x * y;
        var basePart = Math.Pow(fx + y, 2);
        double a;

        if (xy > 0)
        {
            var radicand = fx * y;
            if (radicand < 0)
            {
                Page2ValidationText.Text = "Подкоренное выражение f(x)*y < 0 для ветки xy > 0.";
                Page2ResultTextBox.Text = string.Empty;
                return;
            }

            a = basePart - Math.Sqrt(radicand);
            Page2ConditionText.Text = "Использована ветка: xy > 0";
        }
        else if (xy < 0)
        {
            a = basePart + Math.Sqrt(Math.Abs(fx * y));
            Page2ConditionText.Text = "Использована ветка: xy < 0";
        }
        else
        {
            a = basePart + 1;
            Page2ConditionText.Text = "Использована ветка: xy = 0";
        }

        Page2ResultTextBox.Text = a.ToString("G17", CultureInfo.InvariantCulture);
    }

    private void ClearPage2Click(object? sender, RoutedEventArgs e)
    {
        Page2XTextBox.Text = string.Empty;
        Page2YTextBox.Text = string.Empty;
        FxSinhRadio.IsChecked = true;
        Page2ResultTextBox.Text = string.Empty;
        Page2ConditionText.Text = string.Empty;
        Page2ValidationText.Text = string.Empty;
    }

    private void CalculatePage3Click(object? sender, RoutedEventArgs e)
    {
        Page3ValidationText.Text = string.Empty;

        if (!TryParse(Page3X0TextBox.Text, out var x0) ||
            !TryParse(Page3XkTextBox.Text, out var xk) ||
            !TryParse(Page3DxTextBox.Text, out var dx) ||
            !TryParse(Page3ATextBox.Text, out var a) ||
            !TryParse(Page3BTextBox.Text, out var b))
        {
            Page3ValidationText.Text = "Введите корректные числовые значения x0, xk, dx, a, b.";
            Page3ResultTextBox.Text = string.Empty;
            BuildEmptyChart();
            return;
        }

        if (Math.Abs(dx) < 1e-12)
        {
            Page3ValidationText.Text = "Шаг dx не может быть равен 0.";
            Page3ResultTextBox.Text = string.Empty;
            BuildEmptyChart();
            return;
        }

        if (x0 < xk && dx < 0)
        {
            Page3ValidationText.Text = "Для x0 < xk шаг dx должен быть положительным.";
            return;
        }

        if (x0 > xk && dx > 0)
        {
            Page3ValidationText.Text = "Для x0 > xk шаг dx должен быть отрицательным.";
            return;
        }

        var points = new List<Point>();
        var sb = new StringBuilder();
        sb.AppendLine("#      x                  y");

        var index = 1;
        var x = x0;
        const int maxIterations = 100000;
        var iterations = 0;

        while (true)
        {
            if (dx > 0 && x > xk + 1e-12)
            {
                break;
            }

            if (dx < 0 && x < xk - 1e-12)
            {
                break;
            }

            var y = CalculateThirdFunctionY(x, a, b);
            points.Add(new Point(x, y));
            sb.AppendLine($"{index,-6} {x, -18:G12} {y, -18:G12}");

            x += dx;
            index++;
            iterations++;

            if (iterations >= maxIterations)
            {
                Page3ValidationText.Text = "Слишком много точек. Увеличьте шаг dx или сузьте диапазон.";
                break;
            }
        }

        if (points.Count == 0)
        {
            Page3ValidationText.Text = "Не удалось построить точки. Проверьте диапазон и шаг dx.";
            Page3ResultTextBox.Text = string.Empty;
            BuildEmptyChart();
            return;
        }

        Page3ResultTextBox.Text = sb.ToString();
        BuildChart(points);
    }

    private void ClearPage3Click(object? sender, RoutedEventArgs e)
    {
        Page3X0TextBox.Text = string.Empty;
        Page3XkTextBox.Text = string.Empty;
        Page3DxTextBox.Text = string.Empty;
        Page3ATextBox.Text = string.Empty;
        Page3BTextBox.Text = string.Empty;
        Page3ResultTextBox.Text = string.Empty;
        Page3ValidationText.Text = string.Empty;
        BuildEmptyChart();
    }

    private static double CalculateThirdFunctionY(double x, double a, double b)
    {
        return x + Math.Sqrt(Math.Abs(Math.Pow(x, 3) + a - b * Math.Exp(x)));
    }

    private void BuildChart(IReadOnlyList<Point> points)
    {
        FunctionChart.SetPoints(points);
    }

    private void BuildEmptyChart()
    {
        FunctionChart.Clear();
    }

    private async void OnClosingWithConfirmation(object? sender, CancelEventArgs e)
    {
        if (_exitConfirmed)
        {
            return;
        }

        e.Cancel = true;
        var confirmed = await ConfirmExitAsync();

        if (!confirmed)
        {
            return;
        }

        _exitConfirmed = true;
        Close();
    }

    private Task<bool> ConfirmExitAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        var yesButton = new Button
        {
            Content = "Да",
            Width = 100
        };
        ToolTip.SetTip(yesButton, "Подтвердить закрытие приложения");

        var noButton = new Button
        {
            Content = "Нет",
            Width = 100
        };
        ToolTip.SetTip(noButton, "Отменить закрытие приложения");

        var dialog = new Window
        {
            Title = "Подтверждение выхода",
            Width = 380,
            Height = 180,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Вы действительно хотите выйти из приложения?",
                        TextWrapping = TextWrapping.Wrap
                    },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Spacing = 10,
                        Children = { noButton, yesButton }
                    }
                }
            }
        };

        yesButton.Click += (_, _) =>
        {
            tcs.TrySetResult(true);
            dialog.Close();
        };

        noButton.Click += (_, _) =>
        {
            tcs.TrySetResult(false);
            dialog.Close();
        };

        dialog.Closed += (_, _) =>
        {
            if (!tcs.Task.IsCompleted)
            {
                tcs.TrySetResult(false);
            }
        };

        _ = dialog.ShowDialog(this);
        return tcs.Task;
    }
}
