namespace CrowdedBackend.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RaspberryPi
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RaspberryPiID { get; set; }

    [ForeignKey("Venue")]
    public int VenueID { get; set; }

    public int raspX { get; set; }
    public int raspY { get; set; }

    // Navigation property
    public Venue Venue { get; set; }
}