using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLogic.Data;
using BLogic.Models.Contracts;
using BLogic.Models.Clients;
using Microsoft.AspNetCore.Authorization;
using BLogic.Models;
using BLogic.Models.Advisors;

namespace BLogic.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contract.Include(c => c.Client).Include(ac => ac.AdvisorContracts).ThenInclude(a => a.Advisor).ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ContractId,EvidenceNumber,ClosureDate,ValidityDate")] Contract contract, 
            [Bind("ClientId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Client client,
            [Bind("AdvisorId,FirstName,LastName,Email,Phone,BirthNumber,Age")]Advisor advisor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                contract.Client = client;
                _context.Add(contract); //vyřešit pokud v db již poradce je!!!
                _context.Add(advisor); //vyřešit přidání vícero poradců!!!!!
                _context.Add(new AdvisorContract { Advisor = advisor, AdvisorId = advisor.AdvisorId, Contract = contract, ContractId = contract.ContractId });
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract.Where(c => c.ContractId == id).Include(cl => cl.Client).Include(ac => ac.AdvisorContracts).ThenInclude(a => a.Advisor).FirstOrDefaultAsync(m => m.ContractId == id);
            
   
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ContractId,EvidenceNumber,ClosureDate,ValidityDate")] Contract contract,
            [Bind("ClientId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Client client)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { // button na přidání poradce?
                    contract.Client = client;
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
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
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contract.FindAsync(id);
            _context.Contract.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.ContractId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdvisor(int id, int ContractId)
        {
            var advisor = await _context.Advisor.FindAsync(id);
            //tady je to potřeba dořešit nějaké try catch?
            var contract = await _context.Contract.Where(c => c.ContractId == ContractId).Include(ac => ac.AdvisorContracts).ThenInclude(a => a.Advisor).FirstOrDefaultAsync(m => m.ContractId == ContractId);
            foreach (var advor in contract.AdvisorContracts
                .Where(a => a.ContractId == ContractId).Where(i => i.AdvisorId == id))
            {
                contract.AdvisorContracts.Remove(advor);
            }
            await _context.SaveChangesAsync();
            

            return RedirectToAction("Edit",new {id = ContractId});
        }
    }
}
