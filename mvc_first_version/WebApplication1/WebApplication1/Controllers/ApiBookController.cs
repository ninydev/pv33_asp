using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiBookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiBook
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/ApiBook/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookModel>> GetBookModel(int id)
        {
            var bookModel = await _context.Books.FindAsync(id);

            if (bookModel == null)
            {
                return NotFound();
            }

            return bookModel;
        }

        // PUT: api/ApiBook/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookModel(int id, BookModel bookModel)
        {
            if (id != bookModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookModelExists(id))
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

        // POST: api/ApiBook
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookModel>> PostBookModel(BookModel bookModel)
        {
            _context.Books.Add(bookModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookModel", new { id = bookModel.Id }, bookModel);
        }

        // DELETE: api/ApiBook/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookModel(int id)
        {
            var bookModel = await _context.Books.FindAsync(id);
            if (bookModel == null)
            {
                return NotFound();
            }

            _context.Books.Remove(bookModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookModelExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
