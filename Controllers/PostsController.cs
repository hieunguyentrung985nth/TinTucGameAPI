using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models.View;

namespace TinTucGameAPI.Controllers
{
    [Authorize(Roles ="Staff, Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly doan5Context _context;

        public PostsController(doan5Context context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Post>> GetPost(string id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(string id, CreatePostViewModel post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }
            Post model = await _context.Posts.Where(p => p.Id == id).Include(c => c.Categories).FirstOrDefaultAsync();
            model.Title = post.Title;
            model.Description = post.Description;
            model.Content = post.Content;
            model.Status = "pending";
            model.Slug = post.Slug;
            model.View = post.View;
            //model = new Post
            //{
            //    Id = post.Id,
            //    CreatedAt = post.CreatedAt,
            //    UpdatedAt = DateTime.Now,
            //    Status = "pending",
            //    Author = post.Author,
            //    Title = post.Title,
            //    Description = post.Description,
            //    Content = post.Content,
            //    Slug = post.Slug,
            //    View = post.View
            //};
            model.UpdatedAt = DateTime.Now;
            model.Categories.Clear();
            await _context.SaveChangesAsync();

            foreach (var cate in post.Categoryids)
            {
                var category = _context.Categories.Find(cate);
                model.Categories.Add(category);

            }

            //_context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(CreatePostViewModel post)
        {
            var claims = this.User.Identity as ClaimsIdentity;
            var userId = claims.FindFirst("Id")?.Value;
            Post model = new Post
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = "pending",
                Author = userId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Banner = post.Banner,
                Slug = post.Slug,
                View = 0
            };
            foreach (var cate in post.Categoryids)
            {
                var category = _context.Categories.Find(cate);
                model.Categories.Add(category);
            }

            _context.Posts.Add(model);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PostExists(model.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPost", new { id = model.Id }, model);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(string id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
        [HttpPost("{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadBanner(string id)
        {
            var formData = Request.Form.Files.FirstOrDefault();
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound("Không thấy bài viết");
            }
            if (formData != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(formData.FileName);
                var path = Directory.GetCurrentDirectory() + "/Uploads/Posts";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var filestream = new FileStream(path + "/" + fileName, FileMode.Create))
                {
                    await formData.CopyToAsync(filestream);
                }
                post.Banner = fileName;
                await _context.SaveChangesAsync();
            }
            return Ok(new
            {
                success = 1
            });
        }
        [AllowAnonymous]
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload-images")]
        public async Task<IActionResult> UploadImages()
        {
            var formData = Request.Form.Files.FirstOrDefault();
            if (formData != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(formData.FileName);
                var path = Directory.GetCurrentDirectory() + "/Uploads/Posts";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var filestream = new FileStream(path + "/" + fileName, FileMode.Create))
                {
                    await formData.CopyToAsync(filestream);
                }
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    path = $@"https://localhost:44354/contents/{fileName}"
                });
            }
            return Ok(new
            {
                failed = 1
            });
        }
        [HttpGet]
        [Route("my-posts")]
        public async Task<IActionResult> GetMyPosts(int page)
        {
            var claims = this.User.Identity as ClaimsIdentity;
            var userId = claims.FindFirst("Id")?.Value;
            var posts = await _context.Posts.Where(p => p.Author == userId).CountAsync();
            int pageSize = 5;
            int currentPage = page;
            var totalItems = posts;
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = await _context.Posts.Where(p => p.Author == userId).OrderByDescending(p => p.UpdatedAt).ThenByDescending(p=>p.CreatedAt).Skip((currentPage - 1) * pageSize).Take(pageSize).Include(p => p.Categories).Include(p => p.Feeds).ToListAsync();
            return Ok(new
            {
                data = data,
                pager = pager
            });
        }
        [HttpGet]
        [Route("approve-posts")]
        public async Task<IActionResult> GetPosts(int page)
        {
            var posts = await _context.Posts.Where(p => p.Status == "pending").Include(p => p.AuthorNavigation).Include(p => p.Categories).OrderByDescending(p => p.UpdatedAt).ThenByDescending(p => p.CreatedAt).Select(p => new { p.Id, p.Title, p.Description, p.Content, p.AuthorNavigation.staff, p.CreatedAt, p.UpdatedAt, p.View, p.Status, p.Slug, p.Banner, categoryids = p.Categories.Select(c => c.Id) }).ToListAsync();
            //int pageSize = 5;
            //int currentPage = page;
            //var totalItems = posts;
            //var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            //if (currentPage > totalPages) currentPage = totalPages;
            //if (currentPage < 1) currentPage = 1;
            //Pager pager = new Pager(totalItems, currentPage, pageSize);
            //var data = await _context.Posts.Where(p => p.Author == userId).OrderByDescending(p => p.UpdatedAt).ThenByDescending(p => p.CreatedAt).Skip((currentPage - 1) * pageSize).Take(pageSize).Include(p => p.Categories).Include(p => p.Feeds).ToListAsync();
            return Ok(new
            {
                data = posts
            });
        }
        [HttpPost]
        [Route("approve-post")]
        public async Task<IActionResult> ApprovePost(Dictionary<string,string> form)
        {
            //var data = Request.Form["id"].ToString();
            var id = form["id"].ToString();
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound("không tìm thấy bài");
            post.Status = "live";
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost]
        [Route("decline-post/{id}")]
        public async Task<IActionResult> DeclinePost(string id, Feed feed)
        {
            //var data = Request.Form["id"].ToString();
            //var id = form["id"].ToString();
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound("không tìm thấy bài");
            post.Status = "decline";
            post.UpdatedAt = DateTime.Now;
            var oldFeed = _context.Feeds.Where(f => f.PostId == feed.PostId).FirstOrDefault();
            if(oldFeed == null)
            {
                feed.Id = Guid.NewGuid().ToString();
                _context.Feeds.Add(feed);

            }
            else
            {
                oldFeed.Content = feed.Content;
                oldFeed.UserId = feed.UserId;
            }
          
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet]
        [Route("all-posts")]
        public async Task<IActionResult> GetAllPosts(int page)
        {
            var posts = await _context.Posts.Where(p => p.Status == "live").CountAsync();
            int pageSize = 5;
            int currentPage = page;
            var totalItems = posts;
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = await _context.Posts.Where(p => p.Status == "live").Include(p => p.AuthorNavigation).Include(p => p.Categories).OrderByDescending(p => p.UpdatedAt).ThenByDescending(p => p.CreatedAt).Select(p => new { p.Id, p.Title, p.Description, p.Content, p.AuthorNavigation.staff, p.CreatedAt, p.UpdatedAt, p.View, p.Status, p.Slug, p.Banner, categoryids = p.Categories.Select(c => c.Id) }).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new
            {
                data = data,
                pager = pager
            });
        }
        [HttpPut]
        [Route("update-post")]
        public async Task<IActionResult> UpdatePost(string id, CreatePostViewModel post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }
            Post model = await _context.Posts.Where(p => p.Id == id).Include(c => c.Categories).FirstOrDefaultAsync();
            model.Title = post.Title;
            model.Description = post.Description;
            model.Content = post.Content;
            model.Status = "live";
            model.Slug = post.Slug;
            model.View = post.View;
            //model = new Post
            //{
            //    Id = post.Id,
            //    CreatedAt = post.CreatedAt,
            //    UpdatedAt = DateTime.Now,
            //    Status = "pending",
            //    Author = post.Author,
            //    Title = post.Title,
            //    Description = post.Description,
            //    Content = post.Content,
            //    Slug = post.Slug,
            //    View = post.View
            //};
            model.UpdatedAt = DateTime.Now;
            model.Categories.Clear();
            await _context.SaveChangesAsync();

            foreach (var cate in post.Categoryids)
            {
                var category = _context.Categories.Find(cate);
                model.Categories.Add(category);

            }

            //_context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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
        [HttpDelete]
        [Route("delete-post")]
        public async Task<IActionResult> PostDelete(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
    }

}
