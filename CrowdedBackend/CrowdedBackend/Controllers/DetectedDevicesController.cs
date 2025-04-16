using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;

namespace CrowdedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectedDevicesController : ControllerBase
    {
        private readonly MyDbContext _context;
        private CircleUtils circleUtils;
        private readonly ILogger<DetectedDevicesController> _logger;


        public DetectedDevicesController(MyDbContext context, ILogger<DetectedDevicesController> logger)
        {
            _context = context;
            circleUtils = new CircleUtils();
            _logger = logger;
        }

        // GET: api/DetectedDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetectedDevice>>> GetDetectedDevice()
        {
            return await _context.DetectedDevice.ToListAsync();
        }

        // GET: api/DetectedDevices/getHeatmapAtSpecificTime/17891909
        [HttpGet("getHeatmapAtSpecificTime/{timestamp}")]
        public async Task<ActionResult<String>> GetDetectedDevice(int timestamp)
        {
            // TODO: We should find the closest timestamp to the given
            var detectedDevices = await _context.DetectedDevice
                .Where(d => d.timestamp.Equals(timestamp)).ToListAsync();
   
            if (detectedDevices.IsNullOrEmpty())
            {
                return NotFound();
            }
            
            List<(float x, float y)> listOfDeviceLocations = [];
            foreach (var detectedDevice in detectedDevices)
            {
                listOfDeviceLocations.Add(((float) detectedDevice.deviceX,(float) detectedDevice.deviceY));
            }

            Venue venue = detectedDevices[0].Venue;

            List<(float x, float y)> raspLocations = [];

            foreach (var rasp in venue.RaspberryPis)
            {
                raspLocations.Add(((float) rasp.raspX, (float) rasp.raspY));
            }
            
            String heatmapBase64Encoded = HeatmapGenerator.Generate(venue.VenueName, raspLocations, listOfDeviceLocations);

            return heatmapBase64Encoded;
        }

        // PUT: api/DetectedDevices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetectedDevice(int id, DetectedDevice detectedDevice)
        {
            if (id != detectedDevice.detectedDeviceId)
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
        
            return CreatedAtAction("GetDetectedDevice", new { id = detectedDevice.detectedDeviceId }, detectedDevice);
        }

        // POST: api/DetectedDevices/uploadMultiple
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("uploadMultiple")]
        public async Task<ActionResult<DetectedDevice>> PostDetectedDevices(RaspOutputData raspOutputData)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Console.WriteLine(raspOutputData.ToJson());
            
            var raspberryPi = await _context.RaspberryPi.FindAsync(raspOutputData.Id);
            
            if (raspberryPi == null)
            {
                return Problem("Raspberry Pi not found", statusCode: 500);
            }
            
            if (circleUtils.addData(raspOutputData, new Point(raspberryPi.raspX, raspberryPi.raspY)) == 3)
            {
                var points = circleUtils.CalculatePosition();
                foreach (var point in points)
                {
                    _context.Add(null);
                }
                
                await _context.SaveChangesAsync();
                circleUtils.wipeData();
            }
            
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
            return _context.DetectedDevice.Any(e => e.detectedDeviceId == id);
        }

    }
}
