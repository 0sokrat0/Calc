using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Variant11Avalonia.Controls;

/// <summary>
/// Пользовательский элемент управления для отображения графика по набору точек.
/// </summary>
public class FunctionChartControl : UserControl
{
    private readonly Canvas _canvas;

    /// <summary>
    /// Инициализирует новый экземпляр элемента управления графиком.
    /// </summary>
    public FunctionChartControl()
    {
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

        _canvas = new Canvas
        {
            Background = new SolidColorBrush(Color.Parse("#FAFAFA")),
            MinHeight = 320,
            MinWidth = 400,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };

        Content = _canvas;
        SizeChanged += (_, _) => RedrawIfNeeded();
        AttachedToVisualTree += (_, _) => RedrawIfNeeded();

        Clear();
    }

    /// <summary>
    /// Получает последний набор точек, отображенных на графике.
    /// </summary>
    public IReadOnlyList<Point> LastPoints { get; private set; } = Array.Empty<Point>();

    /// <summary>
    /// Передает точки для последующей отрисовки графика.
    /// </summary>
    /// <param name="points">Набор вычисленных точек.</param>
    public void SetPoints(IReadOnlyList<Point> points)
    {
        LastPoints = points
            .Where(p => !double.IsNaN(p.X) && !double.IsNaN(p.Y) && !double.IsInfinity(p.X) && !double.IsInfinity(p.Y))
            .ToList();
        RedrawIfNeeded();
    }

    private void RedrawIfNeeded()
    {
        if (LastPoints.Count == 0)
        {
            return;
        }

        if (_canvas.Bounds.Width <= 1 || _canvas.Bounds.Height <= 1)
        {
            Dispatcher.UIThread.Post(DrawPoints, DispatcherPriority.Background);
            return;
        }

        DrawPoints();
    }

    private void DrawPoints()
    {
        if (LastPoints.Count == 0)
        {
            return;
        }

        _canvas.Children.Clear();

        var points = LastPoints;
        var canvasWidth = _canvas.Bounds.Width > 1 ? _canvas.Bounds.Width : (Bounds.Width > 1 ? Bounds.Width : 900);
        var canvasHeight = _canvas.Bounds.Height > 1 ? _canvas.Bounds.Height : (Bounds.Height > 1 ? Bounds.Height : 320);

        const double leftMargin = 60;
        const double rightMargin = 20;
        const double topMargin = 25;
        const double bottomMargin = 40;

        var plotWidth = Math.Max(1, canvasWidth - leftMargin - rightMargin);
        var plotHeight = Math.Max(1, canvasHeight - topMargin - bottomMargin);

        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);

        if (Math.Abs(maxX - minX) < 1e-12)
        {
            minX -= 1;
            maxX += 1;
        }

        if (Math.Abs(maxY - minY) < 1e-12)
        {
            minY -= 1;
            maxY += 1;
        }

        var frame = new Rectangle
        {
            Width = plotWidth,
            Height = plotHeight,
            Stroke = Brushes.LightGray,
            StrokeThickness = 1
        };
        _canvas.Children.Add(frame);
        Canvas.SetLeft(frame, leftMargin);
        Canvas.SetTop(frame, topMargin);

        var xAxis = new Line
        {
            StartPoint = new Point(leftMargin, topMargin + plotHeight),
            EndPoint = new Point(leftMargin + plotWidth, topMargin + plotHeight),
            Stroke = Brushes.DimGray,
            StrokeThickness = 1.2
        };
        _canvas.Children.Add(xAxis);

        var yAxis = new Line
        {
            StartPoint = new Point(leftMargin, topMargin),
            EndPoint = new Point(leftMargin, topMargin + plotHeight),
            Stroke = Brushes.DimGray,
            StrokeThickness = 1.2
        };
        _canvas.Children.Add(yAxis);

        var polyline = new Polyline
        {
            Stroke = Brushes.SteelBlue,
            StrokeThickness = 2
        };

        foreach (var point in points)
        {
            var screenX = leftMargin + (point.X - minX) / (maxX - minX) * plotWidth;
            var screenY = topMargin + (maxY - point.Y) / (maxY - minY) * plotHeight;
            polyline.Points.Add(new Point(screenX, screenY));
        }

        _canvas.Children.Add(polyline);

        var xLabel = new TextBlock { Text = $"x: {minX:G4} .. {maxX:G4}", Foreground = Brushes.Gray };
        var yLabel = new TextBlock { Text = $"y: {minY:G4} .. {maxY:G4}", Foreground = Brushes.Gray };
        var title = new TextBlock { Text = "График y(x)", FontWeight = FontWeight.SemiBold, Foreground = Brushes.Black };

        _canvas.Children.Add(title);
        _canvas.Children.Add(xLabel);
        _canvas.Children.Add(yLabel);

        Canvas.SetLeft(title, leftMargin);
        Canvas.SetTop(title, 2);
        Canvas.SetLeft(xLabel, leftMargin);
        Canvas.SetTop(xLabel, topMargin + plotHeight + 6);
        Canvas.SetLeft(yLabel, leftMargin + 180);
        Canvas.SetTop(yLabel, topMargin + plotHeight + 6);
        _canvas.InvalidateVisual();
    }

    /// <summary>
    /// Очищает текущий график и выводит подсказку-заглушку.
    /// </summary>
    public void Clear()
    {
        LastPoints = Array.Empty<Point>();
        _canvas.Children.Clear();

        var hint = new TextBlock
        {
            Text = "После вычисления здесь появится линия функции",
            Foreground = Brushes.Gray
        };

        _canvas.Children.Add(hint);
        Canvas.SetLeft(hint, 10);
        Canvas.SetTop(hint, 10);
    }
}
