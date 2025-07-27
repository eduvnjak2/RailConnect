using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailConnect.Data;
using RailConnect.Models;

namespace RailConnect.Controllers
{
    public class StanicaPutovanjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PutovanjeController> _logger;


        public StanicaPutovanjeController(ApplicationDbContext context, ILogger<PutovanjeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public IActionResult BackToPutovanja()
        {
            return RedirectToAction(nameof(PutovanjeController.Index), "Putovanje");
        }



        public async Task<IActionResult> Index(int? putovanjeId, int? mjestoPolaska)
        {
            if (putovanjeId == null)
            {
                return NotFound();
            }

            // Retrieve StanicaPutovanje data from the database
            var stanicePutovanja = await _context.StanicaPutovanje
                                                  .Where(sp => sp.IdPutovanje == putovanjeId)
                                                  .ToListAsync();

            // Fetch the necessary StanicaDolazak data
            var stanicaDolazakIds = stanicePutovanja.Select(sp => sp.IdStanicaDolazak).ToList();
            var stanicaDolazakMap = await _context.StanicaDolazak
                                                   .Where(sd => stanicaDolazakIds.Contains(sd.IdStanicaDolazak))
                                                   .ToDictionaryAsync(sd => sd.IdStanicaDolazak, sd => sd.Naziv);

            // Map the data to the view model
            var stanicaPutovanjeViewModels = stanicePutovanja.Select(sp => new StanicaPutovanjeViewModel
            {
                IdStanicaPutovanje = sp.IdStanicaPutovanje,
                IdStanicaDolazak = sp.IdStanicaDolazak,
                IdPutovanje = sp.IdPutovanje,
                StanicaDolazakNaziv = stanicaDolazakMap.ContainsKey(sp.IdStanicaDolazak) ? stanicaDolazakMap[sp.IdStanicaDolazak] : "Unknown"
            }).ToList();

            // Set ViewBag values
            ViewBag.PutovanjeId = putovanjeId;
            ViewBag.MjestoPolaska = mjestoPolaska;

            return View(stanicaPutovanjeViewModels);
        }


        // GET: StanicaPutovanje/Details/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var stanicaPutovanje = await _context.StanicaPutovanje
                .FirstOrDefaultAsync(m => m.IdStanicaPutovanje == id);
            if (stanicaPutovanje == null)
            {
                return NotFound();
            }
            var putovanjef = await _context.Putovanje.FindAsync(stanicaPutovanje.IdPutovanje);

            ViewBag.PutovanjeId = putovanjef.IdPutovanje;
            ViewBag.MjestoPolaska = putovanjef.MjestoPolaska;
            var stanicaDolazak = await _context.StanicaDolazak
                .FirstOrDefaultAsync(sd => sd.IdStanicaDolazak == stanicaPutovanje.IdStanicaDolazak);

            var viewModel = new StanicaPutovanjeViewModel
            {
                IdStanicaPutovanje = stanicaPutovanje.IdStanicaPutovanje,
                IdStanicaDolazak = stanicaPutovanje.IdStanicaDolazak,
                IdPutovanje = stanicaPutovanje.IdPutovanje,
                StanicaDolazakNaziv = stanicaDolazak?.Naziv // Use null conditional operator in case it's null
            };

            return View(viewModel);
        }


        // GET: StanicaPutovanje/Create/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Create(int? putovanjeId)
        {
            if (putovanjeId == null)
            {
                return NotFound();
            }

            // Attempt to find the Putovanje entity by ID
            var putovanjef = await _context.Putovanje.FindAsync(putovanjeId);

            // Check if the entity exists
            if (putovanjef == null)
            {
                // If not found, return a NotFound result or handle as appropriate for your application
                return NotFound();
            }

            var viewModel = new StanicaPutovanjeViewModel
            {
                IdPutovanje = putovanjeId.Value
            };

            // Prepare the dropdown for the available stations
            ViewData["IdStanicaDolazak"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv");
            ViewBag.PutovanjeId = putovanjef.IdPutovanje;
            ViewBag.MjestoPolaska = putovanjef.MjestoPolaska;

            return View(viewModel);
        }

        // POST: StanicaPutovanje/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [Authorize(Roles = "Administrator, Zaposlenik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StanicaPutovanjeViewModel stanicaPutovanjeViewModel)
        {
            _logger.LogInformation($"Received StanicaPolazakNaziv: {stanicaPutovanjeViewModel.IdPutovanje}");
            _logger.LogInformation($"Received StanicaPolazakNaziv: {stanicaPutovanjeViewModel.StanicaDolazakNaziv}");
            _logger.LogInformation($"Received StanicaPolazakNaziv: {stanicaPutovanjeViewModel.IdStanicaDolazak}");

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"VALID StanicaPolazakNaziv: {stanicaPutovanjeViewModel.IdStanicaDolazak}");

                var stanicaPutovanje = new StanicaPutovanje
                {
                    IdPutovanje = stanicaPutovanjeViewModel.IdPutovanje,
                    IdStanicaDolazak = stanicaPutovanjeViewModel.IdStanicaDolazak
                };

                var putovanjef = await _context.Putovanje.FindAsync(stanicaPutovanje.IdPutovanje);

                // Add the new StanicaPutovanje object to the context
                _context.Add(stanicaPutovanje);
                await _context.SaveChangesAsync();

                // Redirect with parameters using a route to pass them correctly
                return RedirectToAction(nameof(Create), "StanicaPutovanje", new { putovanjeId = stanicaPutovanjeViewModel.IdPutovanje});
            }

            // If model state is invalid, return to the create view with the current state
            ViewData["IdStanicaDolazak"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", stanicaPutovanjeViewModel.IdStanicaDolazak);
            return View(stanicaPutovanjeViewModel);
        }




        // GET: StanicaPutovanje/Edit/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stanicaPutovanje = await _context.StanicaPutovanje.FindAsync(id);
            if (stanicaPutovanje == null)
            {
                return NotFound();
            }
            var putovanjef = await _context.Putovanje.FindAsync(stanicaPutovanje.IdPutovanje);

            ViewBag.PutovanjeId = putovanjef.IdPutovanje;
            ViewBag.MjestoPolaska = putovanjef.MjestoPolaska;
            var viewModel = new StanicaPutovanjeViewModel
            {
                IdStanicaPutovanje = stanicaPutovanje.IdStanicaPutovanje,
                IdStanicaDolazak = stanicaPutovanje.IdStanicaDolazak,
                IdPutovanje = stanicaPutovanje.IdPutovanje,
                StanicaDolazakNaziv = _context.StanicaDolazak
                                      .Where(s => s.IdStanicaDolazak == stanicaPutovanje.IdStanicaDolazak)
                                      .Select(s => s.Naziv)
                                      .FirstOrDefault()
            };

            ViewData["IdStanicaDolazak"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", stanicaPutovanje.IdStanicaDolazak);
            return View(viewModel);
        }

        // POST: StanicaPutovanje/Edit/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStanicaPutovanje,IdStanicaDolazak,IdPutovanje,StanicaDolazakNaziv")] StanicaPutovanjeViewModel viewModel)
        {
            var putovanjef = await _context.Putovanje.FindAsync(viewModel.IdPutovanje);

            if (id != viewModel.IdStanicaPutovanje)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var stanicaPutovanje = new StanicaPutovanje
                    {
                        IdStanicaPutovanje = viewModel.IdStanicaPutovanje,
                        IdStanicaDolazak = viewModel.IdStanicaDolazak,
                        IdPutovanje = viewModel.IdPutovanje
                    };

                    _context.Update(stanicaPutovanje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StanicaPutovanjeExists(viewModel.IdStanicaPutovanje))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "StanicaPutovanje", new { putovanjeId = viewModel.IdPutovanje, mjestoPolaska = putovanjef.MjestoPolaska });
            }
            ViewData["IdStanicaDolazak"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.IdStanicaDolazak);
            return View(viewModel);
        }

        private bool StanicaPutovanjeExists(int id)
        {
            return _context.StanicaPutovanje.Any(e => e.IdStanicaPutovanje == id);
        }


        // GET: StanicaPutovanje/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stanicaPutovanje = await _context.StanicaPutovanje
                .FirstOrDefaultAsync(m => m.IdStanicaPutovanje == id);
            if (stanicaPutovanje == null)
            {
                return NotFound();
            }
            var putovanjef = await _context.Putovanje.FindAsync(stanicaPutovanje.IdPutovanje);

            ViewBag.PutovanjeId = putovanjef.IdPutovanje;
            ViewBag.MjestoPolaska = putovanjef.MjestoPolaska;

            var viewModel = new StanicaPutovanjeViewModel
            {
                IdStanicaPutovanje = stanicaPutovanje.IdStanicaPutovanje,
                IdStanicaDolazak = stanicaPutovanje.IdStanicaDolazak,
                IdPutovanje = stanicaPutovanje.IdPutovanje,
                StanicaDolazakNaziv = await _context.StanicaDolazak
                    .Where(s => s.IdStanicaDolazak == stanicaPutovanje.IdStanicaDolazak)
                    .Select(s => s.Naziv)
                    .FirstOrDefaultAsync()
            };

            return View(viewModel);
        }

        // POST: StanicaPutovanje/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stanicaPutovanje = await _context.StanicaPutovanje.FindAsync(id);
            var putovanjef = await _context.Putovanje.FindAsync(stanicaPutovanje.IdPutovanje);

            if (stanicaPutovanje != null)
            {
                _context.StanicaPutovanje.Remove(stanicaPutovanje);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "StanicaPutovanje", new { putovanjeId = stanicaPutovanje.IdPutovanje, mjestoPolaska = putovanjef.MjestoPolaska });

        }


    }
}
