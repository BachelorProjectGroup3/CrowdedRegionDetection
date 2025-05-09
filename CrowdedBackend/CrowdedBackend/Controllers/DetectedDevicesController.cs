using CrowdedBackend.Helpers;
using CrowdedBackend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace CrowdedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectedDevicesController : ControllerBase
    {
        private const long TimeInterval = 1 * 60 * 1000;
        private readonly MyDbContext _context;
        private DetectedDeviceHelper _detectedDevicesHelper;
        private readonly IHubContext<DetectedDeviceHub> _hubContext;

        public DetectedDevicesController(MyDbContext context, IHubContext<DetectedDeviceHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            _detectedDevicesHelper = new DetectedDeviceHelper(_context, new CircleUtils(), _hubContext);
        }

        // GET: api/DetectedDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetectedDevice>>> GetDetectedDevice()
        {
            return await _context.DetectedDevice.ToListAsync();
        }

        // GET: api/DetectedDevices/getHeatmapAtSpecificTime/1745562072611
        [HttpGet("getHeatmapAtSpecificTime/{timestamp}")]
        public async Task<ActionResult<String>> GetDetectedDevice(long timestamp)
        {
            // Don't record anything not in x min intervals
            timestamp -= (timestamp % TimeInterval);

            var detectedDevices = await _context.DetectedDevice
                .Where(d => d.Timestamp.Equals(timestamp))
                .ToListAsync();

            if (detectedDevices.IsNullOrEmpty())
            {
                return Problem("Detected devices is null or empty", statusCode: 500);
            }

            return await GetDetectedDeviceTimestampHelper(detectedDevices);
        }

        // GET: api/DetectedDevices/getLatestValidHeatmap
        [HttpGet("getLatestValidHeatmap")]
        public async Task<ActionResult<String>> GetLatestValidHeatmap()
        {
            var detectedDevices = await _context.DetectedDevice
                .GroupBy(x => x.Timestamp)
                .Select(g => g.OrderByDescending(x => x.Timestamp).First())
                .ToListAsync();

            if (detectedDevices.IsNullOrEmpty())
            {
                return Problem("Detected devices is null or empty", statusCode: 500);
            }

            return await GetDetectedDeviceTimestampHelper(detectedDevices);
        }

        private async Task<ActionResult<String>> GetDetectedDeviceTimestampHelper(List<DetectedDevice> detectedDevices)
        {
            List<(float x, float y)> listOfDeviceLocations = [];
            foreach (var detectedDevice in detectedDevices)
            {
                listOfDeviceLocations.Add(((float)detectedDevice.DeviceX, (float)detectedDevice.DeviceY));
            }

            Venue? venue = _context.Venue
                .Where(d => d.VenueID.Equals(detectedDevices[0].VenueID))
                .Include(v => v.RaspberryPis)
                .FirstOrDefault();

            if (venue == null)
            {
                return Problem("Venue in detected device is null", statusCode: 500);
            }

            List<(float x, float y)> raspLocations = [];

            foreach (var rasp in venue.RaspberryPis)
            {
                raspLocations.Add(((float)rasp.RaspX, (float)rasp.RaspY));
            }

            String heatmapBase64Encoded = HeatmapGenerator.Generate(venue.VenueName, raspLocations, listOfDeviceLocations);

            return heatmapBase64Encoded;
        }

        // PUT: api/DetectedDevices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetectedDevice(int id, DetectedDevice detectedDevice)
        {
            if (id != detectedDevice.DetectedDeviceId)
            {
                return BadRequest();
            }

            _context.Entry(detectedDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetectedDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DetectedDevices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DetectedDevice>> PostDetectedDevice(DetectedDevice detectedDevice)
        {
            _context.DetectedDevice.Add(detectedDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetectedDevice", new { id = detectedDevice.DetectedDeviceId }, detectedDevice);
        }

        // POST: api/DetectedDevices/uploadMultiple
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("uploadMultiple")]
        public async Task<ActionResult<DetectedDevice>> PostDetectedDevices(RaspOutputData raspOutputData)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Don't record anything not in x min intervals
            now -= (now % TimeInterval);

            await this._detectedDevicesHelper.HandleRaspPostRequest(raspOutputData, now);

            return CreatedAtAction("GetDetectedDevice", new { timestamp = now });
        }

        // DELETE: api/DetectedDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetectedDevice(int id)
        {
            var detectedDevice = await _context.DetectedDevice.FindAsync(id);
            if (detectedDevice == null)
            {
                return NotFound();
            }

            _context.DetectedDevice.Remove(detectedDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetectedDeviceExists(int id)
        {
            return _context.DetectedDevice.Any(e => e.DetectedDeviceId == id);
        }

    }
}
