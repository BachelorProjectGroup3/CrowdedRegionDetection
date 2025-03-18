using System.Runtime.InteropServices.JavaScript;

namespace CrowdedBackend.Models;

public class CanteenInfo
{
    public int id { get; set; }
    public string canteenName { get; set; }
    // raspPositions list of Json with all raspberry Pi's containing x and y coordinates according to heatmap
    public List<string> raspPositions { get; set; }  
}