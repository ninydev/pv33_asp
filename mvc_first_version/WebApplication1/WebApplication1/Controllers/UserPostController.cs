using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserPost
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.Posts
                    .Include(p => p.Author)
                    .Include(p => p.Tags);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserPost/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // GET: UserPost/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: UserPost/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([
            Bind("Id,Title,Slug,Content,SelectedTagIds")] PostCreateViewModel data)
        {
            if (ModelState.IsValid)
            {
                // Set the author to the current logged-in user
                var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "Не удалось определить текущего пользователя.");
                }
                else
                {
                    var postModel = data.ToPostModel(userId);
                    postModel.AuthorId = userId;

                    // Attach selected tags
                    if (postModel.SelectedTagIds != null && postModel.SelectedTagIds.Count > 0)
                    {
                        var tags = await _context.Tags
                            .Where(t => postModel.SelectedTagIds.Contains(t.Id))
                            .ToListAsync();
                        postModel.Tags = tags;
                    }

                    _context.Add(postModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Repopulate select list on validation error with selected values
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", data.SelectedTagIds);
            return View(data);
        }

        // GET: UserPost/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", postModel.AuthorId);
            return View(postModel);
        }

        // POST: UserPost/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Content,AuthorId,CreatedAt,UpdatedAt")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", postModel.AuthorId);
            return View(postModel);
        }

        // GET: UserPost/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // POST: UserPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel != null)
            {
                _context.Posts.Remove(postModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostModelExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
