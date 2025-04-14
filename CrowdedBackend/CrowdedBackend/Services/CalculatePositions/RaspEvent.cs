using NuGet.Packaging.Signing;

namespace CrowdedBackend.Services.CalculatePositions;

public class RaspEvent
{
    public string macAddress;
    public int signalStrengthRSSI;
    public string company;
    public Timestamp timestamp;

}