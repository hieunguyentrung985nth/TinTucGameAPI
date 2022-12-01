using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TinTucGameAPI.Models;

namespace TinTucGameAPI.Controllers
{
    //[Authorize(Roles ="Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly doan5Context _context;

        public CategoriesController(doan5Context context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int page)
        {
            int pageSize = 3;
            int currentPage = page;
            var totalItems = await _context.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            Pager pager = new Pager(totalItems, currentPage, pageSize);
            var data = (await _context.Categories.Include(c => c.CategoryNavigation).Include(c=>c.InverseCategoryNavigation).ToListAsync()).Where(c => c.Categoryid == null).ToList();
            //var data = await _context.staff.Include(u => u.User).Select(u => new SM
            //{
            //    Staff = u,
            //    Role = u.User.Roles.Where(r => r.Role1 != "User").FirstOrDefault()
            //}).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new
            {
                data = data,
                pager = pager
            });
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("get-all")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var data = await _context.Categories.ToListAsync();
            //var data = await _context.staff.Include(u => u.User).Select(u => new SM
            //{
            //    Staff = u,
            //    Role = u.User.Roles.Where(r => r.Role1 != "User").FirstOrDefault()
            //}).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new
            {
                data = data
            });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(string id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(string id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            category.Id = Guid.NewGuid().ToString();
            _context.Categories.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CategoryExists(category.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _context.Categories
                .Include(c => c.InverseCategoryNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);
            foreach (var children in category.InverseCategoryNavigation)
            {
                children.Categoryid = category.Categoryid;
            }
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(string id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        [HttpGet]
        [Route("all-categories")]
        public async Task<IActionResult> GetCategories()
        {
            var data = await _context.Categories.Where(c => c.Categoryid == null).Include(c=>c.InverseCategoryNavigation).ToListAsync();
            return Ok(new
            {
                data=data
            });
        }
    }
}
