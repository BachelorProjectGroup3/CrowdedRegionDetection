namespace CrowdedBackend.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class Venue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VenueID { get; set; }

    [Required]
    [StringLength(50)]
    public string VenueName { get; set; }

    // Navigation properties
    public ICollection<RaspberryPi> RaspberryPis { get; set; } = new List<RaspberryPi>();
    public ICollection<DetectedDevice> DetectedDevices { get; set; } = new List<DetectedDevice>();
}