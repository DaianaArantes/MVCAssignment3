/*
 * KGSail MVC Application
 *
 * KGTasksController process incoming requests,
 * handle user input and interactions, and execute appropriate application logic.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KGSail.Models;

namespace KGSail.Controllers
{
    public class KGTasksController : Controller
    {
        private readonly SailContext _context;

        public KGTasksController(SailContext context)
        {
            _context = context;
        }

        // GET: KGTasks
        // Returns access to View Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tasks.ToListAsync());
        }

        // GET: KGTasks/Details/5
        // Returns the Details View for a given Id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .SingleOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: KGTasks/Create
        // Returns Create View
        public IActionResult Create()
        {
            return View();
        }

        // POST: KGTasks/Create
        // Creates a new task with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Name,Description")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                TempData["message"] = "Task " + tasks.Name + " was created";
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
        }

        // GET: KGTasks/Edit/5
        // Returns the Edit View for a given Id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.SingleOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        // POST: KGTasks/Edit/5
        // Edits a Task with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,Name,Description")] Tasks tasks)
        {
            if (id != tasks.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Task " + tasks.Name + " was edited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TaskId))
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
            return View(tasks);
        }

        // GET: KGTasks/Delete/5
        // Returns the Delete View for a given Id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .SingleOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: KGTasks/Delete/5
        // Delete a Task with the selected bind fields
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.SingleOrDefaultAsync(m => m.TaskId == id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            TempData["message"] = "Task " + tasks.Name + " was deleted";
            return RedirectToAction(nameof(Index));
        }

        // Verifies if Tasks exist by a given ID
        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
