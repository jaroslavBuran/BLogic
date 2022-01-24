using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLogic.Data;
using BLogic.Models.Advisors;
using BLogic.Models;

namespace BLogic.Controllers
{
    public class AdvisorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Advisors
        public IActionResult Index(string selectedStatus = "")
        {
            var vm = new FilterViewModel();

            var list = new List<SelectListItem>();

            var data = _context.Advisor
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Contract)
                .ThenInclude(c => c.Client)
                .AsQueryable();

            foreach (Advisor advisor in data)
            {
                list.Add(new SelectListItem { Value = advisor.BirthNumber, Text = $"{advisor.FirstName} {advisor.LastName}" });
            }

            vm.Statuses = list;

            if (!String.IsNullOrEmpty(selectedStatus))
            {
                data = data.Where(c => c.BirthNumber == selectedStatus);
            }

            vm.DataAdvisor = data.ToList();

            return View(vm);
        }

        // GET: Advisors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advisor = await _context.Advisor
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.AdvisorId == id);
            if (advisor == null)
            {
                return NotFound();
            }

            return View(advisor);
        }

        // GET: Advisors/Create
        public IActionResult Create(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Advisors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("AdvisorId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Advisor advisor, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                bool isAdvisor = _context.Advisor.Any(bn => bn.BirthNumber == advisor.BirthNumber);

                if (isAdvisor)
                {
                    TempData["Message"] = "Poradce již existuje!";
                    advisor = _context.Advisor.Where(bn => bn.BirthNumber == advisor.BirthNumber).First();
                    return View(advisor);
                }
                else
                {
                    _context.Add(advisor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                if (returnUrl != null)
                {
                    var contract = await _context.Contract
                        .Where(i => i.ContractId == id)
                        .Include(ac => ac.AdvisorContracts)
                        .FirstOrDefaultAsync();

                    if (contract.AdvisorContracts.Any(a => a.AdvisorId == advisor.AdvisorId))
                    {
                        TempData["Message"] = "Poradce je již u smlouvy evidován!";
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        _context.Add(new AdvisorContract { Advisor = advisor, AdvisorId = advisor.AdvisorId, Contract = contract, ContractId = contract.ContractId });
                        await _context.SaveChangesAsync();
                        return Redirect(returnUrl);
                    } 
                }
            }
            return View(advisor);
        }

        // GET: Advisors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advisor = await _context.Advisor.FindAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }
            return View(advisor);
        }

        // POST: Advisors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdvisorId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Advisor advisor)
        {
            if (id != advisor.AdvisorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advisor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvisorExists(advisor.AdvisorId))
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
            return View(advisor);
        }

        // GET: Advisors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advisor = await _context.Advisor
                .FirstOrDefaultAsync(m => m.AdvisorId == id);
            if (advisor == null)
            {
                return NotFound();
            }

            return View(advisor);
        }

        // POST: Advisors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advisor = await _context.Advisor.FindAsync(id);
            _context.Advisor.Remove(advisor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvisorExists(int id)
        {
            return _context.Advisor.Any(e => e.AdvisorId == id);
        }
    }
}
