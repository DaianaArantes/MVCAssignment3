/*
 * KGSail MVC Application
 * 
 * KGMembershipTypesController process incoming requests,
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
    public class KGMembershipTypesController : Controller
    {
        private readonly SailContext _context;

        public KGMembershipTypesController(SailContext context)
        {
            _context = context;
        }

        // GET: KGMembershipTypes
        // Returns access to View Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.MembershipType.ToListAsync());
        }

        // GET: KGMembershipTypes/Details/5
        // Returns the Details View for a given Id
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .SingleOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // GET: KGMembershipTypes/Create
        // Returns Create View
        public IActionResult Create()
        {
            return View();
        }

        // POST: KGMembershipTypes/Create
        // Creates a new Membership with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipType);
                await _context.SaveChangesAsync();
                TempData["message"] = "Membership Type " + membershipType.MembershipTypeName + " was created";
                return RedirectToAction(nameof(Index));
            }
            return View(membershipType);
        }

        // GET: KGMembershipTypes/Edit/5
        // Returns the Edit View for a given Id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType.FindAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        // POST: KGMembershipTypes/Edit/5
        // Edits a Membership with the selected bind fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            if (id != membershipType.MembershipTypeName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipType);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Membership Type " + membershipType.MembershipTypeName + " was edited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipTypeExists(membershipType.MembershipTypeName))
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
            return View(membershipType);
        }

        // GET: KGMembershipTypes/Delete/5
        // Returns the Delete View for a given Id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .SingleOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // POST: KGMembershipTypes/Delete/5
        // Delete a Membership with the selected bind fields
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var membershipType = await _context.MembershipType.SingleOrDefaultAsync(m => m.MembershipTypeName == id);
            _context.MembershipType.Remove(membershipType);
            await _context.SaveChangesAsync();
            TempData["message"] = "Membership Type " + membershipType.MembershipTypeName + " was deleted";
            return RedirectToAction(nameof(Index));
        }

        // Verifies if a MembershipType exists by a given ID
        private bool MembershipTypeExists(string id)
        {
            return _context.MembershipType.Any(e => e.MembershipTypeName == id);
        }
    }
}
