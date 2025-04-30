using System.ComponentModel.DataAnnotations;
using CrowdedBackend.Services.CalculatePositions;

namespace CrowdedBackend.Models;
using System.Text.Json.Serialization;

public class RaspOutputData
{
    [JsonPropertyName("MachineID")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("Events")]
    public List<RaspEvent> Events { get; set; }
}