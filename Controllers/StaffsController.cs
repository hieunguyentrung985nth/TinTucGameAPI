using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models.View;

namespace TinTucGameAPI.Controllers
{
    [Authorize(Policy ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly doan5Context _context;

        public StaffsController(doan5Context context)
        {
            _context = context;
        }
        class SM
        {
            public staff Staff { get; set; }
            public Role Role { get; set; }
        }
        // GET: api/Staffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<staff>>> Getstaff(int page)
        {
            int pageSize = 2;
            int currentPage = page;
            var totalItems = await _context.staff.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            //var data = await _context.staff.Include(u => u.User).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            var data = await _context.staff.Include(u => u.User).Select(u => new SM
            {
                Staff = u,
                Role = u.User.Roles.Where(r => r.Role1 != "User").FirstOrDefault()
            }).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new
            {
                pager = pager,
                data = data
            });
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
        public async Task<ActionResult<staff>> Poststaff(StaffViewModel staff)
        {
            //var staff = model.ToObject<staff>();
            //var authenticate = model.ToObject<AuthenticateModel>();
            staff.Id = Guid.NewGuid().ToString();
            staff model = new staff()
            {
                Id = staff.Id,
                Name = staff.Name,
                Address = staff.Address,
                Gender = staff.Gender,
                Birthdate = staff.Birthdate,
                Phone = staff.Phone,
                UserId = staff.UserId,
            };
            _context.staff.Add(model);
            var user =await _context.Users.Where(u=>u.Id == staff.UserId).Include(r=>r.Roles).FirstOrDefaultAsync();
            var role = _context.Roles.Where(r => r.Role1 == staff.Job).FirstOrDefault();
            user.Roles.Add(role);
            
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
        [NonAction]
        public string GenerateSalt(int size)
        {
            var saltBytes = new byte[size];
            using (var provider = RandomNumberGenerator.Create())
            {
                provider.GetNonZeroBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        [NonAction]
        public string ComputeHash(byte[] bytesToHash, byte[] salt)
        {
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }
    }
}
