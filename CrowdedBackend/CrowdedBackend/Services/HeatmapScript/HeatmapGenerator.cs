using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using SkiaSharp;


public class HeatmapGenerator
{
    private const int ImageSize = 800;
    private const int GridResolution = 200;
    private const float MapXMin = 0;
    private const float MapXMax = 12;
    private const float MapYMin = 0;
    private const float MapYMax = 12;


    public static String Generate(string venueName, List<(float x, float y)> raspberryPiPositions, List<(float x, float y)> peoplePositions)
    {
        List<(float x, float y)> positionList = new List<(float x, float y)>();
        foreach (var position in peoplePositions)
        {
            positionList.Add((float.Round(position.x), float.Round(position.y)));
            Console.WriteLine(position.x);
            Console.WriteLine(position.y);
        }
        Console.WriteLine(positionList.Count);

        string currentDirectory = Directory.GetCurrentDirectory() + "/Services/HeatmapScript/";
        var backgroundPath = Path.Combine(currentDirectory, venueName) + ".png";
        Console.WriteLine(backgroundPath);

        var image = new SKBitmap(ImageSize, ImageSize);
        using var canvas = new SKCanvas(image);
        canvas.Clear(SKColors.White);

        // Draw background
        if (File.Exists(backgroundPath))
        {
            using var bgStream = File.OpenRead(backgroundPath);
            var background = SKBitmap.Decode(bgStream);
            var destRect = new SKRect(0, 0, ImageSize, ImageSize);
            canvas.DrawBitmap(background, destRect);
        }

        // KDE heatmap
        var density = Compute2DKDE(peoplePositions);
        DrawHeatmap(canvas, density);

        // Overlay Raspberry Pi positions
        DrawPoints(canvas, raspberryPiPositions, SKColors.White, SKColors.Black, 10);

        // Overlay people positions (optional)
        DrawPoints(canvas, positionList, SKColors.Red, SKColors.DarkRed, 5);

        using var ms = new MemoryStream();
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
        var pngBytes = ms.ToArray();

        var converted = Convert.ToBase64String(pngBytes);
        return converted;
    }

    private static void DrawHeatmap(SKCanvas canvas, double[,] density)
    {
        double max = 0;
        foreach (var d in density) max = Math.Max(max, d);

        for (int i = 0; i < GridResolution; i++)
        {
            for (int j = 0; j < GridResolution; j++)
            {
                double value = density[i, j] / max;
                SKColor color = GetHeatmapColor(value);

                float x = i * ImageSize / (float)GridResolution;
                float y = ImageSize - (j * ImageSize / (float)GridResolution); // invert Y

                var rect = new SKRect(x, y, x + ImageSize / (float)GridResolution, y + ImageSize / (float)GridResolution);
                using var paint = new SKPaint { Color = color };
                canvas.DrawRect(rect, paint);
            }
        }
    }

    private static SKColor GetHeatmapColor(double value)
    {
        // Jet colormap (rough approximation)
        byte r = (byte)(Math.Min(1.0, 1.5 - Math.Abs(4 * (value - 0.75))) * 255);
        byte g = (byte)(Math.Min(1.0, 1.5 - Math.Abs(4 * (value - 0.5))) * 255);
        byte b = (byte)(Math.Min(1.0, 1.5 - Math.Abs(4 * (value - 0.25))) * 255);
        return new SKColor(r, g, b, 128); // semi-transparent
    }

    private static void DrawPoints(SKCanvas canvas, List<(float x, float y)> points, SKColor fill, SKColor border, float radius)
    {
        foreach (var (x, y) in points)
        {
            float px = (x - MapXMin) / (MapXMax - MapXMin) * ImageSize;
            float py = ImageSize - ((y - MapYMin) / (MapYMax - MapYMin) * ImageSize);

            using var paint = new SKPaint
            {
                Color = border,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };
            canvas.DrawCircle(px, py, radius, paint);

            paint.Color = fill;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(px, py, radius - 2, paint);
        }
    }

    private static double[,] Compute2DKDE(List<(float x, float y)> points)
    {
        double bandwidth = 0.8; // adjust for blur/sharpness
        int N = points.Count;

        double[,] density = new double[GridResolution, GridResolution];

        for (int i = 0; i < GridResolution; i++)
        {
            for (int j = 0; j < GridResolution; j++)
            {
                double x = MapXMin + (MapXMax - MapXMin) * i / GridResolution;
                double y = MapYMin + (MapYMax - MapYMin) * j / GridResolution;

                double sum = 0;
                foreach (var (px, py) in points)
                {
                    double dx = x - px;
                    double dy = y - py;
                    double norm = (dx * dx + dy * dy) / (2 * bandwidth * bandwidth);
                    sum += Math.Exp(-norm);
                }

                density[i, j] = sum / (N * 2 * Math.PI * bandwidth * bandwidth);
            }
        }

        return density;
    }
}
