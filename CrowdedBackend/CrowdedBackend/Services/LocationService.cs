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
        List<Location> locations = this.getIntersection(raspReadings);
        return 1;
    }

    private List<Location> getIntersection(List<RaspReading> raspReadings)
    {
        var x = 0;
        var y = 0;
        foreach (RaspReading raspReading in raspReadings)
        {
            int x1 = raspReading.originLocation.x;
            int y1 = raspReading.originLocation.y;
            // Find where formula 1 and 2 intersect and plot points into formula 3 to check
            // Consider adding a margin of error in case of imperfect circles
            var formula =
                (Math.Pow(x, 2) - 2 * x * x1 + Math.Pow(x1, 2) + Math.Pow(y, 2) - 2 * y * y1 + Math.Pow(y1, 2)) /
                Math.Pow(raspReading.distanceInRSSI, 2);
        }
        return initRaspLocations;
    }
}