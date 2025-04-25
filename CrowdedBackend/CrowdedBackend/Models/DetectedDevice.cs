namespace CrowdedBackend.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DetectedDevice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int detectedDeviceId { get; set; }

    [ForeignKey("Venue")]
    public int venueID { get; set; }

    public double deviceX { get; set; }
    public double deviceY { get; set; }

    [Required]
    public long timestamp { get; set; }

    // Navigation property
    public Venue? Venue { get; set; }
}