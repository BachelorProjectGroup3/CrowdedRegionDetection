namespace CrowdedBackend.Services.CalculatePositions;

public class Point
{
    public double X;
    public double Y;
    public List<int> ParentIndex = new();
    public double Angle;

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}