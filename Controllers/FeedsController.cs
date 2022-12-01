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
    [Authorize(Roles ="Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        private readonly doan5Context _context;

        public FeedsController(doan5Context context)
        {
            _context = context;
        }

        // GET: api/Feeds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feed>>> GetFeeds()
        {
            return await _context.Feeds.ToListAsync();
        }

        // GET: api/Feeds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feed>> GetFeed(string id)
        {
            var feed = await _context.Feeds.FindAsync(id);

            if (feed == null)
            {
                return NotFound();
            }

            return feed;
        }

        // PUT: api/Feeds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeed(string id, Feed feed)
        {
            if (id != feed.Id)
            {
                return BadRequest();
            }

            _context.Entry(feed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedExists(id))
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

        // POST: api/Feeds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Feed>> PostFeed(Feed feed)
        {
            _context.Feeds.Add(feed);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FeedExists(feed.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFeed", new { id = feed.Id }, feed);
        }

        // DELETE: api/Feeds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeed(string id)
        {
            var feed = await _context.Feeds.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }

            _context.Feeds.Remove(feed);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedExists(string id)
        {
            return _context.Feeds.Any(e => e.Id == id);
        }
    }
}
