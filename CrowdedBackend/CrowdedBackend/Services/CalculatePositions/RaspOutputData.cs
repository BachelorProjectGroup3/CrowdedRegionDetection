using NuGet.Packaging.Signing;

namespace CrowdedBackend.Services.CalculatePositions;

public class RaspOutputData
{
    public int id;
    public string macAddress;
    public int signalStrengthRSSI;
    public string company;
    public Timestamp timestamp;
}