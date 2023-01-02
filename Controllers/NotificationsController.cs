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
        public async Task<IActionResult> GetNotifications(string id, int page=1)
        {
            int pageSize = 8;
            var data = await _context.Notifications
                .Join(_context.Posts, n => n.PostId, p => p.Id, (n, p) => new { not = n, post = p })
                .Join(_context.staff, np => np.post.Author, s => s.UserId, (np, s) => new { nps = np, author = s })
                .Where(nps => nps.nps.not.OwnerId == id)
                .OrderByDescending(nps => nps.nps.not.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(nps => new { nps.nps.not, post = nps.nps.post, author = nps.author })
                .ToListAsync();
            //var data = from notification in _context.Notifications
            //           join posts in _context.Posts on notification.PostId equals posts.Id into postsNot
            //           from np in postsNot.DefaultIfEmpty()
            //           join author in _context.staff on np.AuthorNavigation.Id equals author.UserId into AA
            //           from Ab in AA.DefaultIfEmpty()
            //           select new
            //           {
            //               not = notification,
            //               post = np,
            //               author = Ab
            //           };
            var totalUnreads = await _context.Notifications
                .Where(n => n.OwnerId == id && n.Read == "false")
                .CountAsync();
            return Ok(new { data = data, totalUnreads = totalUnreads });
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
                model.OwnerId = user.Id;
                _context.Notifications.Add(model);
                await _context.SaveChangesAsync();
            }
            var post = await _context.Posts.FindAsync(model.PostId);
            var author = await _context.staff.Where(s => s.UserId == post.Author).FirstOrDefaultAsync();
            return Ok(new
            {
                not = model,
                post = post,
                author = author
            });
        }
        [HttpGet]
        [Route("read-notification")]
        public async Task<IActionResult> ReadNotifications(string id)
        {
            var model = await _context.Notifications.FindAsync(id);
            if (model.Read == "true") model.Read = "false";
            else
            model.Read = "true";
            await _context.SaveChangesAsync();
            return Ok(model);
        }
    }
}
