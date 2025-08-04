using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [Authorize]
    public class ToDoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ToDoesController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ToDoes
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var todos = await _context.ToDos
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.IsDone)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(todos);
        }

        // GET: ToDoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (toDo == null)
                return NotFound();

            return View(toDo);
        }

        // GET: ToDoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                toDo.UserId = _userManager.GetUserId(User);
                toDo.IsDone = false;
                toDo.CreatedAt = DateTime.Now;
                toDo.CompletedAt = null;

                _context.Add(toDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(toDo);
        }

        // GET: ToDoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (toDo == null)
                return NotFound();

            return View(toDo);
        }

        // ✅ POST: ToDoes/Edit/5 — FULLY UPDATED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsDone,CreatedAt,CompletedAt")] ToDo toDo)
        {
            var existingToDo = await _context.ToDos.FirstOrDefaultAsync(t => t.Id == id);

            if (existingToDo == null || existingToDo.UserId != _userManager.GetUserId(User))
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingToDo.Title = toDo.Title;
                    existingToDo.IsDone = toDo.IsDone;
                    existingToDo.CreatedAt = toDo.CreatedAt;
                    //existingToDo.completedAt = toDo.CompletedAt;
                    existingToDo.CompletedAt = toDo.CompletedAt;


                    if (toDo.IsDone)
                    {
                        // Use provided CompletedAt if valid, otherwise set it now
                        existingToDo.CompletedAt = toDo.CompletedAt ?? DateTime.Now;
                    }
                    else
                    {
                        existingToDo.CompletedAt = null;
                    }

                    _context.Update(existingToDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (toDo == null)
                return NotFound();

            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (toDo != null)
                _context.ToDos.Remove(toDo);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.Id == id);
        }
    }
}
