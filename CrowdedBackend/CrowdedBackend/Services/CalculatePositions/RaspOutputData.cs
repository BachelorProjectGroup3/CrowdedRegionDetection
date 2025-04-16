namespace CrowdedBackend.Services.CalculatePositions;
using System.Text.Json.Serialization;

public class RaspOutputData
{
    [JsonPropertyName("MachineID")]
    public int Id { get; set; }
    
    [JsonPropertyName("Events")]
    public List<RaspEvent> Events { get; set; }
}