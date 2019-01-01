/*
* KGSail MVC Application Assignment 3
*
* KGParkingController process incoming requests,
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
    public class KGParkingController : Controller
    {
        private readonly SailContext _context;

        public KGParkingController(SailContext context)
        {
            _context = context;
        }

        // GET: KGParking
        // Return view with parkings ordered by ParkingCode
        public async Task<IActionResult> Index()
        {
            // Ignores actualBoatId that is empty, because it should not suppose to be updated 
            var sailContext = _context.Parking.Include(p => p.BoatType)
                .Where(s=> s.ActualBoatId != "")
                .OrderBy(s=> s.ParkingCode);
            return View(await sailContext.ToListAsync());
        }

        // GET: KGParking/Details
        // Return view with details about selected parking
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parking
                .Include(p => p.BoatType)
                .SingleOrDefaultAsync(m => m.ParkingCode == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // GET: KGParking/Create
        // Return view with form to create new parking
        public IActionResult Create()
        {

            var boatType = _context.BoatType
                .OrderBy(b => b.Name);

            ViewData["BoatTypeId"] = new SelectList(boatType, "BoatTypeId", "Name");
            return View();
        }

        // POST: KGParking/Create
        // If no errors, save new parking to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParkingCode,BoatTypeId,ActualBoatId")] Parking parking)
        {
            if (parking.ParkingCode == null)
            {
                TempData["message"] = "Please insert a parkingCode";
                ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", parking.BoatTypeId);
                return View(parking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(parking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", parking.BoatTypeId);
            return View(parking);
        }

        // GET: KGParking/Edit
        // Return view if parking information to edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parking.SingleOrDefaultAsync(m => m.ParkingCode == id);

            if (parking == null)
            {
                return NotFound();
            }

            var boatType = _context.BoatType
                .OrderBy(b => b.Name);

            ViewData["BoatTypeId"] = new SelectList(boatType, "BoatTypeId", "Name");
            return View(parking);
        }

        // POST: KGParking/Edit/5
        // If no errors, save editions to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ParkingCode,BoatTypeId,ActualBoatId")] Parking parking)
        {
            if (id != parking.ParkingCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingExists(parking.ParkingCode))
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
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", parking.BoatTypeId);
            return View(parking);
        }

        // GET: KGParking/Delete
        // Show information about selected parking before deleting
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parking
                .Include(p => p.BoatType)
                .SingleOrDefaultAsync(m => m.ParkingCode == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // POST: KGParking/Delete
        // Delete parking from database
        // Return to index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var parking = await _context.Parking.SingleOrDefaultAsync(m => m.ParkingCode == id);
            _context.Parking.Remove(parking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingExists(string id)
        {
            return _context.Parking.Any(e => e.ParkingCode == id);
        }
    }
}
