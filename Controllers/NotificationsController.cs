using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinTucGameAPI.Models;

namespace TinTucGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly doan5Context _context;

        public NotificationsController(doan5Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            return Ok(await _context.Notifications.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> GetNotifications(Notification model)
        {          
            model.Read = "false";
            model.CreatedAt = DateTime.Now;
            var role = await _context.Roles.Where(r => r.Role1 == "Manager").FirstOrDefaultAsync();
            var managers = await _context.staff
                .Include(s => s.User.Roles)
                .Where(s => s.User.Roles.Contains(role))
                .ToListAsync();
            foreach (var user in managers)
            {
                model.Id = Guid.NewGuid().ToString();
                model.OwnerId = user.UserId;
                _context.Notifications.Add(model);
                await _context.SaveChangesAsync();
            }
            return Ok(model);
        }
        [HttpPost]
        [Route("read-notification")]
        public async Task<IActionResult> ReadNotifications(string id)
        {
            var model = await _context.Notifications.FindAsync(id);
            model.Read = "true";
            await _context.SaveChangesAsync();
            return Ok(model);
        }
    }
}
