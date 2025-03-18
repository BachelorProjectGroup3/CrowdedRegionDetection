namespace CrowdedBackend.Structs;

public struct RaspReading
{
    public Location originLocation { get; set; }
    public int distanceInRSSI { get; set; }
}