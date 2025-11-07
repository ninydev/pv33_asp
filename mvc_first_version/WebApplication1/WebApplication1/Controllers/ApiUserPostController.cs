using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Mappers;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUserPostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiUserPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiUserPost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetPosts()
        {
            var applicationDbContext =
                _context.Posts
                    .Include(p => p.Author)
                    .Include(p => p.Tags);
            
            var vm = PostMapper.ToViewModels(await applicationDbContext.ToListAsync());
            
            return Ok(vm);
        }

        // GET: api/ApiUserPost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostEntity>> GetPostEntity(int id)
        {
            var postEntity = await _context.Posts.FindAsync(id);

            if (postEntity == null)
            {
                return NotFound();
            }

            return postEntity;
        }

        // PUT: api/ApiUserPost/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostEntity(int id, PostEntity postEntity)
        {
            if (id != postEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(postEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostEntityExists(id))
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

        // POST: api/ApiUserPost
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostEntity>> PostPostEntity(PostEntity postEntity)
        {
            _context.Posts.Add(postEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostEntity", new { id = postEntity.Id }, postEntity);
        }

        // DELETE: api/ApiUserPost/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostEntity(int id)
        {
            var postEntity = await _context.Posts.FindAsync(id);
            if (postEntity == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(postEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostEntityExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
