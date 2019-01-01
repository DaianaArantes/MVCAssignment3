/*
* KGSail MVC Application Assignment 3
*
* KGMembershipController process incoming requests,
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
    public class KGMembershipController : Controller
    {
        private readonly SailContext _context;

        public KGMembershipController(SailContext context)
        {
            _context = context;
        }

        // GET: KGMembership
        //  Return index view with memberships for a selected member
        public async Task<IActionResult> Index(int? memberId, string fullName)
        {
            if (memberId == null)     
            {
                if (Request.Cookies["memberId"] != null)     // memberId is in a cookie
                {
                    memberId = Convert.ToInt32(Request.Cookies["memberId"]);     
                }
                else     // memberId is not in a cookie     
                {
                    TempData["message"] = "Select a member to see its memberships";
                    return RedirectToAction("Index", "KGMember");
                }
            }     
            else     // memberId is in the query string
            {
                // create/change a cookie called "memberId"
                Response.Cookies.Append("memberId", memberId.ToString());
            }

            if (fullName == null)     
            {
                var member = _context.Member
                    .SingleOrDefault(a => a.MemberId == memberId);
                
		        fullName = member.FullName;
            }
            
	    // create/change a cookie called "fullName"
            Response.Cookies.Append("fullName", fullName);

            // Filter memberships from MemberId selected
            var membershipList = _context.Membership
                .Where(m => m.MemberId == memberId)
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .OrderByDescending(a => a.Year);

            ViewData["Title"] = "Memberships for " + fullName;

            return View(await membershipList.ToListAsync());     // Return list with filtered memberships
        }

        // GET: KGMembership
        // returns details for memberships from a selected member
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .SingleOrDefaultAsync(m => m.MembershipId == id);
            
	        if (membership == null)
            {
                return NotFound();
            }

            // Get fullName to use on title
            string fullName = Request.Cookies["fullName"];

            ViewData["Title"] = "Details of a Membership for " + fullName;

            return View(membership);
        }

        // GET: KGMembership
        // return view with form to create a new membership
        public IActionResult Create()
        {
            var membershipTypeNames = _context.MembershipType
                .OrderBy(a => a.MembershipTypeName);

            string fullName = Request.Cookies["fullName"];   

            ViewData["MemberId"] = Convert.ToInt32(Request.Cookies["memberId"]);
            ViewData["MembershipTypeName"] = new SelectList(membershipTypeNames, "MembershipTypeName", "MembershipTypeName");
            ViewData["CurrentYear"] = DateTime.Now.Year;
	        ViewData["Title"] = "Create a Membership for " + fullName;
            
            return View();
        }

        // POST: KGMembership
        // If no errors, membership is created, and return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
	        var annualFeeStructure = _context.AnnualFeeStructure
                .FirstOrDefault(a => a.Year == DateTime.Now.Year);

	        if (annualFeeStructure == null)
            {
                TempData["message"] = "There is no annual fee for " + DateTime.Now.Year;
                return RedirectToAction(nameof(Index));
            }

            var membershipType = _context.MembershipType
                    .FirstOrDefault(a => a.MembershipTypeName == membership.MembershipTypeName);
               
            //Calculate the fee for selected year and membershiptype
            double value = annualFeeStructure.AnnualFee * membershipType.RatioToFull;

            membership.Fee = value;            

            if (ModelState.IsValid)
            {
                _context.Add(membership);
                await _context.SaveChangesAsync();
                TempData["message"] = "Membership " + membership.MembershipTypeName + " was created";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType, "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // GET: KGMembership
        // Return view with form for a membership to edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership.SingleOrDefaultAsync(a => a.MembershipId == id);
            
	        if (membership == null)
            {
                return NotFound();
            }

            var membershipTypeNames = _context.MembershipType
               .OrderBy(a => a.MembershipTypeName);

            string fullName = Request.Cookies["fullName"];

            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(membershipTypeNames, "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            ViewData["Title"] = "Edit a Membership for " + fullName;

            return View(membership);
        }

        // POST: KGMembership
        // If no errors, editions made to a memebrship is saved
        // Returns to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
            if (Request.Cookies["memberId"] != null)
            {
                var memberId = Convert.ToInt32(Request.Cookies["memberId"]);
                
		        var latestYear = _context.Membership
                    .Where(a => a.MemberId == memberId)
                    .Max(a => a.Year);

		        //Checks if year selected is not a prior year	                
		        if (membership.Year < latestYear)
                {
                    TempData["message"] = "Cannot edit a prior year's record";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (id != membership.MembershipId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membership);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Membership for " + membership.Year + " was edited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipExists(membership.MembershipId))
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

            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType, "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // GET: KGMembership
        // Go to delete view with details about selected membership
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .SingleOrDefaultAsync(a => a.MembershipId == id);
            
	        if (membership == null)
            {
                return NotFound();
            }

            string fullName = Request.Cookies["fullName"];

            ViewData["Title"] = "Delete a Membership for " + fullName;

            return View(membership);
        }

        // POST: KGMembership
        // Delete a membership from database
        // Returns to index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membership = await _context.Membership.SingleOrDefaultAsync(a => a.MembershipId == id);
            _context.Membership.Remove(membership);
            await _context.SaveChangesAsync();
            TempData["message"] = "Membership for " + membership.Year + " was deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipExists(int id)
        {
            return _context.Membership.Any(a => a.MembershipId == id);
        }
    }
}
