/*
* KGSail MVC Application
*
* KGBoatTypeController process incoming requests,
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
    public class KGBoatTypeController : Controller
    {
        private readonly SailContext _context;

        public KGBoatTypeController(SailContext context)
        {
            _context = context;
        }

        // GET: KGBoatType
        // Returns access to View Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatType.ToListAsync());
        }

        // GET: KGBoatType/Details/5
        // Returns the Details View for a given Id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .SingleOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // GET: KGBoatType/Create
        // Returns Create View
        public IActionResult Create()
        {
            return View();
        }

        // POST: KGBoatType/Create
        // Creates a new boat with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boatType);
                await _context.SaveChangesAsync();
                TempData["message"] = "Boat Type " + boatType.Name +  " was created"; 
                return RedirectToAction(nameof(Index));
            }
            return View(boatType);
        }

        // GET: KGBoatType/Edit/5
        // Returns the Edit View for a given Id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType.SingleOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }
            return View(boatType);
        }

        // POST: KGBoatType/Edit/5
        // Edits a Boat with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (id != boatType.BoatTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatType);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Boat Type " + boatType.Name + " was edited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatTypeExists(boatType.BoatTypeId))
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
            return View(boatType);
        }

        // GET: KGBoatType/Delete/5
        // Returns the Delete View for a given Id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .SingleOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // POST: KGBoatType/Delete/5
        // Delete a Boat with the selected bind fields
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boatType = await _context.BoatType.SingleOrDefaultAsync(m => m.BoatTypeId == id);
            _context.BoatType.Remove(boatType);
            await _context.SaveChangesAsync();
            TempData["message"] = "BoatType " + boatType.Name + " was deleted";
            return RedirectToAction(nameof(Index));
        }

        // Verifies if a BoatType exists by a given ID
        private bool BoatTypeExists(int id)
        {
            return _context.BoatType.Any(e => e.BoatTypeId == id);
        }
    }
}
