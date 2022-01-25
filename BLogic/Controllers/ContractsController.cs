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
                list.Add(new SelectListItem { Value = contract.Client.BirthNumber,
                    Text = $"RČ: {contract.Client.BirthNumber} - {contract.Client.FirstName} {contract.Client.LastName}" });
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

        public IActionResult Create()
        {
            var clients = _context.Client.AsQueryable();
            var listClients = new List<SelectListItem>();
            foreach (var client in clients)
            {
                listClients.Add(new SelectListItem
                {
                    Value = client.BirthNumber,
                    Text = $"RČ: {client.BirthNumber} - {client.FirstName} {client.LastName}"
                });
            }

            var advisors = _context.Advisor.AsQueryable();
            var listAdvisors = new List<SelectListItem>();
            foreach (var advisor in advisors)
            {
                listAdvisors.Add(new SelectListItem
                {
                    Value = advisor.BirthNumber,
                    Text = $"RČ: {advisor.BirthNumber} - {advisor.FirstName} {advisor.LastName}"
                });
            }

            ViewBag.Clients = listClients;
            ViewBag.Advisors = listAdvisors;

            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ContractId,EvidenceNumber,ClosureDate,ValidityDate")] Contract contract,
            string ClientBirthNumber, string AdvisorBirthNumber)
        {
            Client client = await _context.Client.Where(b => b.BirthNumber == ClientBirthNumber).SingleAsync();
            Advisor advisor = await _context.Advisor.Where(b => b.BirthNumber == AdvisorBirthNumber).Include(ac => ac.AdvisorContracts).SingleAsync();

            if (ModelState.IsValid)
            {
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
                .Include(cl => cl.Client)
                .Include(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            var list = new List<SelectListItem>();

            var data = _context.Advisor.AsQueryable();

            foreach (Advisor advisor in data)
            {
                list.Add(new SelectListItem { Value = advisor.BirthNumber,
                    Text = $"RČ: {advisor.BirthNumber} - {advisor.FirstName} {advisor.LastName}" });
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
            [Bind("ContractId,EvidenceNumber,ClosureDate,ValidityDate,AdvisorContracts")] Contract contract,
            [Bind("ClientId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Client client,
            [Bind("AdvisorId,FirstName,LastName,Email,Phone,BirthNumber,Age")]Advisor advisor)
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
                    _context.Update(advisor);
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

        // POST: Contracts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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
        
        [HttpPost]
        public IActionResult Add(int ContractId, string BirthNumber)
        {
            var contract = _context.Contract.Where(i => i.ContractId == ContractId).Include(ac => ac.AdvisorContracts).First();
            var advisor = _context.Advisor.Where(bn => bn.BirthNumber == BirthNumber).First();

            bool isAdvisor = contract.AdvisorContracts.Any(a => a.Advisor == advisor);

            if (isAdvisor)
            {
                TempData["Message"] = "Poradce je u této smlouvy již evidován!";
            }
            else
            {
                contract.AdvisorContracts.Add(new AdvisorContract { Advisor = advisor, AdvisorId = advisor.AdvisorId, Contract = contract, ContractId = contract.ContractId });
                _context.SaveChanges();
            }
            
            return RedirectToAction("Edit", new {id = ContractId});
        }
    }
}
