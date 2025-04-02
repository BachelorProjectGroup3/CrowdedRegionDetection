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
    public class RaspberryPiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public RaspberryPiController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/RaspberryPi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaspberryPi>>> GetRaspberryPi()
        {
            return await _context.RaspberryPi.ToListAsync();
        }

        // GET: api/RaspberryPi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaspberryPi>> GetRaspberryPi(int id)
        {
            var raspberryPi = await _context.RaspberryPi.FindAsync(id);

            if (raspberryPi == null)
            {
                return NotFound();
            }

            return raspberryPi;
        }

        // PUT: api/RaspberryPi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRaspberryPi(int id, RaspberryPi raspberryPi)
        {
            if (id != raspberryPi.RaspberryPiID)
            {
                return BadRequest();
            }

            _context.Entry(raspberryPi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RaspberryPiExists(id))
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

        // POST: api/RaspberryPi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RaspberryPi>> PostRaspberryPi(RaspberryPi raspberryPi)
        {
            _context.RaspberryPi.Add(raspberryPi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRaspberryPi", new { id = raspberryPi.RaspberryPiID }, raspberryPi);
        }

        // DELETE: api/RaspberryPi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRaspberryPi(int id)
        {
            var raspberryPi = await _context.RaspberryPi.FindAsync(id);
            if (raspberryPi == null)
            {
                return NotFound();
            }

            _context.RaspberryPi.Remove(raspberryPi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RaspberryPiExists(int id)
        {
            return _context.RaspberryPi.Any(e => e.RaspberryPiID == id);
        }
    }
}
