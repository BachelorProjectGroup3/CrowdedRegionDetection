namespace CrowdedBackend.Models;

public class RaspData
{
    public int id { get; set; }
    public string macAddress { get; set; }
    public int signalStrengthRSSI { get; set; } 
    public string company { get; set; }
    public int timestamp { get; set; }
}