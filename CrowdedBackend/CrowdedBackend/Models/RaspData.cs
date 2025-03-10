namespace CrowdedBackend.Models;

public class RaspData
{
    public int id { get; set; }
    public string macAddress { get; set; }
    public string signalStrengthRSSI { get; set; }  // kan det blive en int???
    public string company { get; set; }
    public int timestamp { get; set; } // If not, then public string timeDate { get; set; }
}