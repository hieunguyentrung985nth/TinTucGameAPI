using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models.View;
using TinTucGameAPI.Services;

namespace TinTucGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly doan5Context _context;
        private readonly IEmailSender _sendMail;
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public HomeController(doan5Context context, IEmailSender sendMail, IConfiguration configuration)
        {
            _context = context;
            _sendMail = sendMail;
            //_userManager = userManager;
            //_signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("five-posts")]
        public async Task<IActionResult> GetFivePosts()
        {
            var data = await _context.Posts.OrderByDescending(p => p.CreatedAt).Where(p=>p.Status=="live").Skip(1).Take(4).ToListAsync();
            var first = await _context.Posts.OrderByDescending(p => p.CreatedAt).Where(p => p.Status == "live").FirstOrDefaultAsync();
            return Ok(new
            {
                data=data,
                first=first,
            });
        }
        [HttpGet]
        [Route("all-posts")]
        public async Task<IActionResult> GetFivePosts(int page)
        {
            int currentPage = page;
            int pageSize = 5;
            var totalItems = await _context.Posts.Where(p => p.Status == "live").CountAsync() - 5;
            var totalPages =(int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            else if (currentPage < 0) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = await _context.Posts.OrderByDescending(p => p.CreatedAt).Where(p => p.Status == "live").Skip((currentPage - 1) * pageSize + 5).Take(pageSize).ToListAsync();
           
            return Ok(new
            {
                data = data,
                pager = pager,
            });
        }
        [HttpGet]
        [Route("most-view")]
        public async Task<IActionResult> GetMostViewPosts()
        {         
            var data = await _context.Posts
                .OrderByDescending(p => p.View)
                .Where(p => p.Status == "live")
                .Where(p=>p.CreatedAt > DateTime.Now.AddDays(-7))
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                data = data,
            });
        }
        [HttpGet]
        [Route("month-post")]
        public async Task<IActionResult> GetMonthPosts()
        {
            var data = await _context.Posts
                .OrderByDescending(p => p.View)
                .Where(p => p.Status == "live")
                .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))
                .Take(6)
                .ToListAsync();

            return Ok(new
            {
                data = data,
            });
        }
        [HttpGet]
        [Route("post-category")]
        public async Task<IActionResult> GetCategoryPost(string category)
        {
            var data = await _context.Posts
                .OrderByDescending(p => p.View)
                .Where(p => p.Status == "live")
                .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))
                .Take(6)
                .ToListAsync();

            return Ok(new
            {
                data = data,
            });
        }
        [HttpGet]
        [Route("detail")]
        public async Task<IActionResult> GetPost(string slug)
        {
            var data = await _context.Posts
                .Where(p => p.Status == "live")
                .Where(p => p.Slug == slug)
                .Include(p=>p.AuthorNavigation.staff)
                .Include(p=>p.Categories)
                .Select(p=> new {p,categoryids= p.Categories.Select(c=>c.Id)})
                .FirstOrDefaultAsync();
            data.p.View++;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                data = data,
            });
        }
        [HttpGet]
        [Route("category-posts")]
        public async Task<IActionResult> GetCategoryPosts(string slug, int page)
        {
            var category = await _context.Categories.Where(c => c.Slug == slug).FirstOrDefaultAsync();
            int currentPage = page;
            int pageSize = 5;
            var totalItems = await _context.Posts
                .Where(p=>p.Status == "live")
                .Where(p=>p.Categories.Contains(category))
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            else if (currentPage < 0) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = await _context.Posts
                .Where(p => p.Status=="live")
                .Where(p => p.Categories.Contains(category))
                .Skip((currentPage - 1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(new
            {
                data = data,
                pager=pager
            });
        }
        [HttpGet]
        [Route("search-post")]
        public async Task<IActionResult> SearchPost(string search, int page)
        {
            int currentPage = page;
            int pageSize = 5;
            var totalItems = await _context.Posts
                .Where(p => p.Status == "live")
                .Where(p => p.Title.Trim().ToLower().Contains(search.Trim().ToLower()))
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            else if (currentPage < 0) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            if (totalItems != 0)
            {
                var data = await _context.Posts
                .Where(p => p.Status == "live")
                .Where(p => p.Title.Trim().ToLower().Contains(search.Trim().ToLower()))
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                return Ok(new
                {
                    data = data,
                    pager = pager
                });
            }
            return Ok(new
            {
                data = ""
            });
        }
        [HttpPost]
        [Route("more-post")]
        public async Task<IActionResult> MorePost(Dictionary<string,object> form)
        {
            Random r = new Random();
            var k = JsonConvert.DeserializeObject<string[]>(form["categoryids"].ToJson());
            int page = int.Parse(form["page"].ToString());
            int currentPage = page;
            int pageSize = 1;
            var f = await _context.Posts
                .Where(p => p.Status == "live").Include(p=>p.Categories).ToListAsync();
            var totalItems =f
                .Where(p => p.Categories.Select(c => c.Id).Contains(k[0]))
                .Count();
            var test = f.Select(c =>new {s= c.Categories.Select(c => c.Id), c.Title }).ToList();
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            else if (currentPage < 0) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = f
                 .Where(p => p.Categories.Select(c => c.Id).Contains(k[0]))
                 .Skip((currentPage - 1) * pageSize)
                 .Take(pageSize)
                 .Select(p => new {p.Title, p.Content, p.Description, p.AuthorNavigation, p.View, p.CreatedAt, p.Banner, p.Slug})
                 .ToList();
            return Ok(new
            {
                data = data,
                pager = pager
            });
        }


      

        [HttpGet("send-mail")]
        public async Task<IActionResult> SendMail()
        {
            await _sendMail.SendEmailAsync("staff1@gmail.com", "Hello", "test");
            return Ok();

        }

        [HttpGet("send-sms")]
        public async Task<IActionResult> SendSmS()
        {
            await _sendMail.SendSmsAsync("0125487954", "Hello");
            return Ok();

        }
    }
}
