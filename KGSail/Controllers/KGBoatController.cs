/*
* KGSail MVC Application Assignment 3
*
* KGBoatController process incoming requests,
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
using Microsoft.AspNetCore.Http;

namespace KGSail.Controllers
{
    public class KGBoatController : Controller
    {
        private readonly SailContext _context;

        public KGBoatController(SailContext context)
        {
            _context = context;
        }

        // GET: KGBoat
        // Return view with boats ordered by boatClass
        public async Task<IActionResult> Index(int? memberId, string fullName)
        {
            if (memberId == null)
            {
                // Check if cookie exists before saving it
                if (Request.Cookies["memberId"] != null)
                {
                    memberId = Convert.ToInt16(Request.Cookies["memberId"]);
                }
                else
                {
                    // if there is no memberId in the cookie
                    TempData["message"] = "Please select a member to see their boats";
                    return RedirectToAction("Index", "KGMember");
                }
            }
            else
            {
                // Create cookie for memberId
                Response.Cookies.Append("memberId", memberId.ToString());
            }

            if (fullName == null)
            {
                var member = _context.Member
                    .SingleOrDefault(m=> m.MemberId == memberId);

                fullName = member.FullName;
            }

            // create/change a cookie called "fullName"
            Response.Cookies.Append("fullName", fullName);


            var sailContext = _context.Boat
                .Where(b => b.MemberId == memberId)
                .Include(b => b.BoatType).Include(b => b.Member).Include(b => b.ParkingCodeNavigation)
                .OrderBy(r => r.BoatClass);

            // Get name to put on title
            var memberName = _context.Member
                    .SingleOrDefault(m => m.MemberId == memberId);

            ViewData["Title"] = "Boats for " + memberName.FullName;

            return View(await sailContext.ToListAsync());
        }

        // GET: KGBoat/Details/5
        // Return view with details about selected boat
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .SingleOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }

            var memberId = Convert.ToInt16(Request.Cookies["memberId"]);

            // Get name to put on title
            var memberName = _context.Member
                    .SingleOrDefault(m => m.MemberId == memberId);

            ViewData["Title"] = "Boats details for " + memberName.FullName;
            return View(boat);
        }

        // GET: KGBoat/Create
        // Return view with form to create new boat
        public IActionResult Create()
        {
           
            // gets parking codes that are not null
            var boatParkingCode = _context.Boat
                .Select(b=> b.ParkingCode)
                .Where(b => b != null)
                .ToList();

            // validates with parking code does not contains on boat tatle, so it can be displayed to be used
            var parking = _context.Parking
                .Where(p => !boatParkingCode.Contains(p.ParkingCode))
                .Select(b => b.ParkingCode)
                .ToList();

            // Add empty spot
            parking.Add(null);
            
            // order by name to use on the boatTypeId dropdown
            var boatType = _context.BoatType
                .OrderBy(b => b.Name);

            // get member id 
            var memberId = Convert.ToInt16(Request.Cookies["memberId"]);

            // Get name to put on title
            var memberName = _context.Member
                    .SingleOrDefault(m => m.MemberId == memberId);

            ViewData["Title"] = "Add Boat for " + memberName.FullName;

            ViewData["BoatTypeId"] = new SelectList(boatType, "BoatTypeId", "Name");
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName");
            ViewData["ParkingCode"] = new SelectList(parking, "ParkingCode");
            return View();
        }

        // POST: KGBoat/Create
        // If no errors, save new boat to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["ParkingCode"] = new SelectList(_context.Parking, "ParkingCode", "ParkingCode", boat.ParkingCode);
            return View(boat);
        }

        // GET: KGBoat
        // Return view if boat information to edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat.SingleOrDefaultAsync(m => m.BoatId == id);


            if (boat == null)
            {
                return NotFound();
            }

            // order by name
            var boatType = _context.BoatType
                .OrderBy(b => b.Name);


            // lists only parking spaces of the same boat-type
            var boatParkingCode = _context.Boat
                .Where(b=> b.ParkingCode != boat.ParkingCode)
                .Select(b => b.ParkingCode)
                .Where(b => b != null)
                .ToList();

            // displays only parking spaces not being used alredy
            var parkingSpace = _context.Parking
               .Where(b => !boatParkingCode.Contains(b.ParkingCode))
               
               .ToList();

            var memberId = Convert.ToInt16(Request.Cookies["memberId"]);

            // Get name to put on title
            var memberName = _context.Member
                    .SingleOrDefault(m => m.MemberId == memberId);

            ViewData["Title"] = "Edit a Boat for " + memberName.FullName;

            ViewData["BoatTypeId"] = new SelectList(boatType, "BoatTypeId", "Name");
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["ParkingCode"] = new SelectList(parkingSpace, "ParkingCode", "ParkingCode", boat.ParkingCode);
            return View(boat);
        }

        // POST: KGBoat/Edit
        // If no errors, save editions to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            if (id != boat.BoatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatExists(boat.BoatId))
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
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["ParkingCode"] = new SelectList(_context.Parking, "ParkingCode", "ParkingCode", boat.ParkingCode);
            return View(boat);
        }

        // GET: KGBoat/Delete/5
        // Show information about selected boat before deleting
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .SingleOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }


            // Get name to put on title
            var memberName = _context.Member
                    .SingleOrDefault(m => m.MemberId == boat.MemberId);

            ViewData["Title"] = "Delete a Boat for " + memberName.FullName;

            return View(boat);
        }

        // POST: KGBoat/Delete
        // Delete boat from database
        // Return to index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boat = await _context.Boat.SingleOrDefaultAsync(m => m.BoatId == id);
            _context.Boat.Remove(boat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatExists(int id)
        {
            return _context.Boat.Any(e => e.BoatId == id);
        }
    }
}
