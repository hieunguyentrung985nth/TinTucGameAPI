using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinTucGameAPI.Models;

namespace TinTucGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly doan5Context _context;
        private readonly Models2.doan5Context2 _context2;

        public AdminController(doan5Context context, Models2.doan5Context2 context2)
        {
            _context = context;
            _context2 = context2;
        }
        [HttpGet]
        [Route("statistical")]
        public async Task<IActionResult> Statistical()
        {
            var data = await _context2.Users
                            .Include(u => u.Posts)
                            .Join(_context2.staff, u=>u.Id, s=>s.UserId, (u, s) => new {user=u,staff=s})
                            .Where(us=>us.user.Roles.Any(r=>r.Role1 == "Staff"))
                            .Select(us => new
                            {
                                us.staff.Name,us.user.Posts.Count
                            })
                            .ToListAsync();
            return Ok(data);
        }
        [HttpGet]
        [Route("statistical-by-month")]
        public async Task<IActionResult> Statistical(int month)
        {
            var data = await _context2.Users
                            .Include(u => u.Posts)
                            .Join(_context2.staff, u => u.Id, s => s.UserId, (u, s) => new { user = u, staff = s })
                             .Where(us => us.user.Roles.Any(r => r.Role1 == "Staff"))
                             .Select(us => new
                             {
                                 us.staff.Name,
                                 count =us.user.Posts.Where(p => p.CreatedAt.Value.Month == month).Count()
                             })
                            .ToListAsync();
            return Ok(data);
        }
    }
}
