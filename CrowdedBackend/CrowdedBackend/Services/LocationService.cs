using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using CrowdedBackend.Structs;
    
namespace CrowdedBackend.Services;

public class LocationService
{

    private List<Location> initRaspLocations;

    public LocationService(List<JsonArray> raspLocations)
    {
        foreach (string raspLocation in raspLocations) 
        {
            JObject json = JObject.Parse(raspLocation);
            initRaspLocations.Add(json.ToObject<Location>());
        }
    }
    
    public int locateDevice(List<RaspReading> raspReadings)
    {
        Location locations = this.getIntersection(raspReadings);
        return 1;
    }

    private Location getIntersection(List<RaspReading> raspReadings)
    {
        raspReadings = this.normalizeRSSI(raspReadings);
        // Assuming there are more raspberry Pi's than three
        var x = 0;
        var y = 0;
        foreach (RaspReading raspReading in raspReadings)
        {
            int x11 = raspReading.originLocation.x;
            int y11 = raspReading.originLocation.y;
            // Find where formula 1 and 2 intersect and plot points into formula 3 to check
            // Consider adding a margin of error in case of imperfect circles
            var formula =
                (Math.Pow(x, 2) - 2 * x * x11 + Math.Pow(x11, 2) + Math.Pow(y, 2) - 2 * y * y11 + Math.Pow(y11, 2)) /
                Math.Pow(raspReading.distanceInRSSI, 2);
        }
        
        
        // Assuming there is an intersection, this can be used. This will probably not work, since there likely is no
        // intersection between the assumed perfect circles
        int x1 = raspReadings[0].originLocation.x;
        int y1 = raspReadings[0].originLocation.y;
        int r1 = raspReadings[0].distanceInRSSI;
        int x2 = raspReadings[1].originLocation.x;
        int y2 = raspReadings[1].originLocation.y;
        int r2 = raspReadings[1].distanceInRSSI;
        int x3 = raspReadings[2].originLocation.x;
        int y3 = raspReadings[2].originLocation.y;
        int r3 = raspReadings[2].distanceInRSSI;

        // Compute the numerator and denominator for y
        int numeratorY = (x2 - x3) * ((x2 * x2 - x1 * x1) + (y2 * y2 - y1 * y1) + (r1 * r1 - r2 * r2))
                            - (x1 - x2) * ((x3 * x3 - x2 * x2) + (y3 * y3 - y2 * y2) + (r2 * r2 - r3 * r3));

        int denominatorY = 2 * ((y1 - y2) * (x2 - x3) - (y2 - y3) * (x1 - x2));

        // Compute y
        int finalY = numeratorY / denominatorY;

        // Compute the numerator and denominator for x
        int numeratorX = (y2 - y3) * ((y2 * y2 - y1 * y1) + (x2 * x2 - x1 * x1) + (r1 * r1 - r2 * r2))
                            - (y1 - y2) * ((y3 * y3 - y2 * y2) + (x3 * x3 - x2 * x2) + (r2 * r2 - r3 * r3));

        int denominatorX = 2 * ((x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2));

        // Compute x
        int finalX = numeratorX / denominatorX;
        
        return new Location() { x = finalX, y = finalY };
        
    }

    private List<RaspReading> normalizeRSSI(List<RaspReading> raspReadings)
    {
        // normalize RSSI to be kind of non exponential???
        return raspReadings;
    }
}