/*
* KGSail MVC Application Assignment 3
*
* KGAnnualFeeStructureController process incoming requests,
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
    public class KGAnnualFeeStructureController : Controller
    {
        private readonly SailContext _context;

        public KGAnnualFeeStructureController(SailContext context)
        {
            _context = context;
        }

        // GET: KGAnnualFeeStructure
        // Returns index view, ordered by year
        public async Task<IActionResult> Index()
        {
            return View(await _context.AnnualFeeStructure
            .OrderBy(a => a.Year)
            .ToListAsync());
        }

        // GET: KGAnnualFeeStructure
        // Returns view with details about a annual fee
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualFeeStructure = await _context.AnnualFeeStructure
                .SingleOrDefaultAsync(m => m.Year == id);
            
	    if (annualFeeStructure == null)
            {
                return NotFound();
            }

            return View(annualFeeStructure);
        }

        // GET: KGAnnualFeeStructure
        // Return view with form to create a new annual fee for current year
        public IActionResult Create()
        {
            var annualFeeStructure = _context.AnnualFeeStructure
                .OrderByDescending(m => m.Year)
                .FirstOrDefault();

            if (annualFeeStructure == null)
            {
                return View();
            }

            annualFeeStructure.Year = DateTime.Now.Year;
            return View(annualFeeStructure);
        }

        // POST: KGAnnualFeeStructure
        // If no errors, saves new annual fee
        // Returns to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Year,AnnualFee,EarlyDiscountedFee,EarlyDiscountEndDate,RenewDeadlineDate,TaskExemptionFee,SecondBoatFee,ThirdBoatFee,ForthAndSubsequentBoatFee,NonSailFee,NewMember25DiscountDate,NewMember50DiscountDate,NewMember75DiscountDate")] AnnualFeeStructure annualFeeStructure)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(annualFeeStructure);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Annual Fee " + annualFeeStructure.Year + " was created";
                    return RedirectToAction(nameof(Index));
                }
			
                return View(annualFeeStructure);	
            }
            catch (Exception ex)
            {

                TempData["message"] = "An error ocorred, please try again";
                return View();
            }
        }

        // GET: KGAnnualFeeStructure
        // Returns view with form with information to edit
        public async Task<IActionResult> Edit(int? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var annualFeeStructure = await _context.AnnualFeeStructure
                .SingleOrDefaultAsync(a => a.Year == id);

            if (annualFeeStructure == null)
            {
                return NotFound();
            }
            
	    return View(annualFeeStructure);
        }

        // POST: KGAnnualFeeStructure
        // If no errors, save changes made to annual fee
        // Returns to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Year,AnnualFee,EarlyDiscountedFee,EarlyDiscountEndDate,RenewDeadlineDate,TaskExemptionFee,SecondBoatFee,ThirdBoatFee,ForthAndSubsequentBoatFee,NonSailFee,NewMember25DiscountDate,NewMember50DiscountDate,NewMember75DiscountDate")] AnnualFeeStructure annualFeeStructure)
        {
            var latestYear = _context.AnnualFeeStructure
               .Max(a => a.Year);

            ////Checks if year selected is not a prior year
            if (id < latestYear)
            {
                TempData["message"] = "Cannot edit a prior year's record";
                return RedirectToAction(nameof(Index));
            }
            
	    if (id != annualFeeStructure.Year)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(annualFeeStructure);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Annual Fee " + annualFeeStructure.Year + " was edited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnualFeeStructureExists(annualFeeStructure.Year))
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
            
	    return View(annualFeeStructure);
        }

        // GET: KGAnnualFeeStructure
        // Go to delete view with details about selected annual fee
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var annualFeeStructure = await _context.AnnualFeeStructure
                    .SingleOrDefaultAsync(m => m.Year == id);
                
		if (annualFeeStructure == null)
                {
                    return NotFound();
                }

                return View(annualFeeStructure);
            }
            catch (Exception ex)
            {
                TempData["message"] = "An error ocorred, please try again";
                return View();
            }
        }

        // POST: KGAnnualFeeStructure
        // Delete a annual fee from database
        // Returns to index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annualFeeStructure = await _context.AnnualFeeStructure.SingleOrDefaultAsync(a => a.Year == id);
            _context.AnnualFeeStructure.Remove(annualFeeStructure);
            await _context.SaveChangesAsync();
            TempData["message"] = "Annual Fee " + annualFeeStructure.Year + " was deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool AnnualFeeStructureExists(int id)
        {
            return _context.AnnualFeeStructure.Any(e => e.Year == id);
        }
    }
}
