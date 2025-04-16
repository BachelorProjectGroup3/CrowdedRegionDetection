using System.Text.Json.Serialization;
using NuGet.Packaging.Signing;

namespace CrowdedBackend.Services.CalculatePositions;

public class RaspEvent
{
    [JsonPropertyName("Address")]

    public required string MacAddress { get; set; }
    public required int Rssi { get; set; }
    public required long UnixTimestamp { get; set; }

}