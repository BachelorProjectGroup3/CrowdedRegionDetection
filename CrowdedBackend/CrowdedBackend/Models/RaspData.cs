using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdedBackend.Models;

public class RaspData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required string MacAddress { get; set; }

    [ForeignKey("RaspberryPi")]
    public required int RaspId { get; set; }

    public required int Rssi { get; set; }

    public required long UnixTimestamp { get; set; }
}