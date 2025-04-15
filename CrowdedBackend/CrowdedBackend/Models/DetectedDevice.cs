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
    public DateTime timestamp { get; set; }

    // Navigation property
    public Venue Venue { get; set; }


    public DetectedDevice(int venueID, double deviceX, double deviceY, DateTime timestamp)
    {
        this.venueID = venueID;
        this.deviceX = deviceX;
        this.deviceY = deviceY;
        this.timestamp = timestamp;
    }
}