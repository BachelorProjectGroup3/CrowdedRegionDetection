using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;

namespace CrowdedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectedDevicesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DetectedDevicesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/DetectedDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetectedDevice>>> GetDetectedDevice()
        {
            return await _context.DetectedDevice.ToListAsync();
        }

        // GET: api/DetectedDevices/17891909
        [HttpGet("{timestamp}")]
        public async Task<ActionResult<DetectedDevice>> GetDetectedDevice(int timestamp)
        {
            var detectedDevice = await _context.DetectedDevice.FindAsync(timestamp);

            if (detectedDevice == null)
            {
                return NotFound();
            }

            return detectedDevice;
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
