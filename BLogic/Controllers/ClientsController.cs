using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLogic.Data;
using BLogic.Models.Clients;
using BLogic.Models;

namespace BLogic.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public IActionResult Index(string selectedStatus = "")
        {
            var vm = new FilterViewModel();

            var list = new List<SelectListItem>();

            var data = _context.Client
                .Include(c => c.Contracts)
                .AsQueryable();

            foreach (Client client in data)
            {
                list.Add(new SelectListItem { Value = client.BirthNumber, Text = $"{client.FirstName} {client.LastName}" });
            }

            vm.Statuses = list;

            if (!String.IsNullOrEmpty(selectedStatus))
            {
                data = data.Where(c => c.BirthNumber == selectedStatus);
            }

            vm.DataClient = data.ToList();

            return View(vm);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.Contracts)
                .ThenInclude(ac => ac.AdvisorContracts)
                .ThenInclude(a => a.Advisor)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Client client, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool isClient = _context.Client.Any(bn => bn.BirthNumber == client.BirthNumber);

                if (isClient)
                {
                    TempData["Message"] = "Klient již existuje!";
                    client = _context.Client.Where(bn => bn.BirthNumber == client.BirthNumber).First();
                    return View(client);
                }
                else
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,FirstName,LastName,Email,Phone,BirthNumber,Age")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Client.FindAsync(id);
            var contracts = _context.Contract.Where(c => c.Client == client).DefaultIfEmpty();

            if (contracts.First() != null)
            {
                foreach (var contract in contracts)
                {
                    _context.Contract.Remove(contract);
                }
            }
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
    }
}
