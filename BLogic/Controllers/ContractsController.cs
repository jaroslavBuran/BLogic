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
        public IActionResult Index(string selectedStatus = "")
        {
            var vm = new FilterViewModel();

            var list = new List<SelectListItem>();

            var data = _context.Contract
                .Include(c => c.Client)
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
                .AsQueryable();

            foreach (Contract contract in data)
            {
                list.Add(new SelectListItem { Value = contract.Client.BirthNumber, Text = $"{contract.Client.FirstName} {contract.Client.LastName}" });
            }

            vm.Statuses = list;

            if (!String.IsNullOrEmpty(selectedStatus))
            {
                data = data.Where(c => c.Client.BirthNumber == selectedStatus);
            }

            vm.DataContract = data.ToList();

            return View(vm);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.Client)
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
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
                //hledám, zda se klient a/nebo poradce již nachází v db
                bool isClient = _context.Client.Any(bn => bn.BirthNumber == client.BirthNumber);
                bool isAdvisor = _context.Advisor.Any(bn => bn.BirthNumber == advisor.BirthNumber);

                if (isClient)
                {
                    client = _context.Client.Where(bn => bn.BirthNumber == client.BirthNumber).First();
                }
                else
                {
                    _context.Add(client);
                }

                if (isAdvisor)
                {
                    advisor = _context.Advisor.Where(bn => bn.BirthNumber == advisor.BirthNumber).First();
                }
                else
                {
                    _context.Add(advisor);
                }


                contract.Client = client;
                _context.Add(contract);
                _context.Add(new AdvisorContract { Advisor = advisor, AdvisorId = advisor.AdvisorId, Contract = contract, ContractId = contract.ContractId });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id, string selectedStatus = "")
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Where(c => c.ContractId == id)
                .Include(cl => cl.Client).Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            var list = new List<SelectListItem>();

            var data = _context.Advisor.AsQueryable();

            foreach (Advisor advisor in data)
            {
                list.Add(new SelectListItem { Value = advisor.BirthNumber, Text = $"{advisor.FirstName} {advisor.LastName}" });
            }

            if (contract == null)
            {
                return NotFound();
            }

            ViewBag.AllAdvisors = list;
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
                {
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
            
            var contract = await _context.Contract.Where(c => c.ContractId == ContractId).Include(ac => ac.AdvisorContracts).ThenInclude(a => a.Advisor).FirstOrDefaultAsync(m => m.ContractId == ContractId);
            foreach (var advor in contract.AdvisorContracts
                .Where(a => a.ContractId == ContractId).Where(i => i.AdvisorId == id))
            {
                contract.AdvisorContracts.Remove(advor);
            }
            await _context.SaveChangesAsync();
            

            return RedirectToAction("Edit",new {id = ContractId});
        }
        /*
        [HttpPost]
        public IActionResult Add(int ContractId, string AdvisorBN)
        {
            var contract = _context.Contract.Where(i => i.ContractId == ContractId).Include(ac => ac.AdvisorContracts).First();
            var advisor = _context.Advisor.Where(bn => bn.BirthNumber == AdvisorBN).First();

            contract.AdvisorContracts.Add(new AdvisorContract { Advisor = advisor, AdvisorId = advisor.AdvisorId, Contract = contract, ContractId = contract.ContractId });
            _context.SaveChanges();

            var after = _context.Contract
                .Where(i => i.ContractId == ContractId)
                .Include(c => c.Client)
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
                .First();

            return View("Edit",after);
        }*/
    }
}
