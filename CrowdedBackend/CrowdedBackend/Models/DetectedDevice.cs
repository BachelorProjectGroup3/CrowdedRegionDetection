namespace CrowdedBackend.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DetectedDevice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetectedDeviceId { get; set; }

    [ForeignKey("Venue")]
    public int VenueID { get; set; }

    public double DeviceX { get; set; }
    public double DeviceY { get; set; }

    [Required]
    public long Timestamp { get; set; }

    // Navigation property
    public Venue? Venue { get; set; }
}