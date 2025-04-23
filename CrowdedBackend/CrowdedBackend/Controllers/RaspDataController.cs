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
    public class RaspDataController : ControllerBase
    {
        private readonly MyDbContext _context;

        public RaspDataController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/RaspData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaspData>>> GetRaspData()
        {
            return await _context.RaspData.ToListAsync();
        }

        // GET: api/RaspData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaspData>> GetRaspData(int id)
        {
            var raspData = await _context.RaspData.FindAsync(id);

            if (raspData == null)
            {
                return NotFound();
            }

            return raspData;
        }

        // PUT: api/RaspData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRaspData(int id, RaspData raspData)
        {
            if (id != raspData.Id)
            {
                return BadRequest();
            }

            _context.Entry(raspData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RaspDataExists(id))
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

        // POST: api/RaspData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RaspData>> PostRaspData(RaspData raspData)
        {
            _context.RaspData.Add(raspData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRaspData", new { id = raspData.Id }, raspData);
        }

        // DELETE: api/RaspData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRaspData(int id)
        {
            var raspData = await _context.RaspData.FindAsync(id);
            if (raspData == null)
            {
                return NotFound();
            }

            _context.RaspData.Remove(raspData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RaspDataExists(int id)
        {
            return _context.RaspData.Any(e => e.Id == id);
        }
    }
}
