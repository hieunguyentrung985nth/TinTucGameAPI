using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinTucGameAPI.Models;

namespace TinTucGameAPI.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly doan5Context _context;

        public StaffsController(doan5Context context)
        {
            _context = context;
        }

        // GET: api/Staffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<staff>>> Getstaff()
        {
            return await _context.staff.ToListAsync();
        }

        // GET: api/Staffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<staff>> Getstaff(string id)
        {
            var staff = await _context.staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return staff;
        }

        // PUT: api/Staffs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putstaff(string id, staff staff)
        {
            if (id != staff.Id)
            {
                return BadRequest();
            }

            _context.Entry(staff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!staffExists(id))
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

        // POST: api/Staffs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<staff>> Poststaff(staff staff)
        {
            staff.Id = Guid.NewGuid().ToString();
            _context.staff.Add(staff);
            staff.User.Id = Guid.NewGuid().ToString();
            _context.Users.Add(staff.User);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (staffExists(staff.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Getstaff", new { id = staff.Id }, staff);
        }

        // DELETE: api/Staffs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletestaff(string id)
        {
            var staff = await _context.staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.staff.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool staffExists(string id)
        {
            return _context.staff.Any(e => e.Id == id);
        }
    }
}
