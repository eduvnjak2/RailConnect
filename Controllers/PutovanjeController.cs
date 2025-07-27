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
    public class PutovanjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PutovanjeController> _logger;

        public PutovanjeController(ApplicationDbContext context, ILogger<PutovanjeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Putovanje
        public async Task<IActionResult> Index(DateTime? filterDate, double? filterPrice, DateTime? previousFilterDate, string sortOrder)
        {
            var today = DateTime.Today;

            // Fetching upcoming travels
            var currentPutovanjaQuery = _context.Putovanje
                .Where(p => p.DatumPolaska > today); // Only include "Putovanje" with a departure date after today

            if (filterDate.HasValue)
            {
                currentPutovanjaQuery = currentPutovanjaQuery.Where(p => p.DatumPolaska.Date == filterDate.Value.Date);
            }

            if (filterPrice.HasValue)
            {
                currentPutovanjaQuery = currentPutovanjaQuery.Where(p => p.Cijena == filterPrice.Value);
            }

            var currentPutovanja = await currentPutovanjaQuery.ToListAsync();

            var stanicePolazak = await _context.StanicaPolazak.ToDictionaryAsync(sp => sp.IdStanicaPolazak, sp => sp.Naziv);
            var staniceDolazak = await _context.StanicaDolazak.ToDictionaryAsync(sd => sd.IdStanicaDolazak, sd => sd.Naziv);

            var currentPutovanjeViewModels = currentPutovanja.Select(p => new PutovanjeViewModel
            {
                IdPutovanje = p.IdPutovanje,
                Voz = p.IdVoz,
                StanicaPolazakNaziv = stanicePolazak.ContainsKey(p.MjestoPolaska) ? stanicePolazak[p.MjestoPolaska] : "Unknown",
                StanicaDolazakNaziv = staniceDolazak.ContainsKey(p.MjestoDolaska) ? staniceDolazak[p.MjestoDolaska] : "Unknown",
                StanicaPolazakId = p.MjestoPolaska,
                Cijena = p.Cijena,
                VrijemePolaska = p.VrijemePolaska,
                VrijemeDolaska = p.VrijemeDolaska,
                DatumPolaska = p.DatumPolaska,
                BrojMjesta = p.BrojMjesta
            }).ToList();

            // Fetching previous travels with average rating filter
            var previousPutovanjaQuery = _context.Putovanje
                .Where(p => p.DatumPolaska < today); // Only include "Putovanje" with a departure date before today

            if (previousFilterDate.HasValue)
            {
                previousPutovanjaQuery = previousPutovanjaQuery.Where(p => p.DatumPolaska.Date == previousFilterDate.Value.Date);
            }

            var previousPutovanja = await previousPutovanjaQuery.ToListAsync();

            var previousPutovanjeViewModels = new List<PutovanjeViewModel>();

            foreach (var p in previousPutovanja)
            {
                // Query to get all reviews for the specific Putovanje
                var recenzije = await _context.Recenzija
                    .Where(r => r.PutovanjeId == p.IdPutovanje)
                    .ToListAsync();

                // Calculate average rating for the reviews
                double averageOcjena = recenzije.Any() ? recenzije.Average(r => r.Ocjena) : 0.0;

                var putovanjeViewModel = new PutovanjeViewModel
                {
                    IdPutovanje = p.IdPutovanje,
                    Voz = p.IdVoz,
                    StanicaPolazakNaziv = stanicePolazak.ContainsKey(p.MjestoPolaska) ? stanicePolazak[p.MjestoPolaska] : "Unknown",
                    StanicaDolazakNaziv = staniceDolazak.ContainsKey(p.MjestoDolaska) ? staniceDolazak[p.MjestoDolaska] : "Unknown",
                    StanicaPolazakId = p.MjestoPolaska,
                    Cijena = p.Cijena,
                    VrijemePolaska = p.VrijemePolaska,
                    VrijemeDolaska = p.VrijemeDolaska,
                    DatumPolaska = p.DatumPolaska,
                    BrojMjesta = p.BrojMjesta,
                    AverageOcjena = averageOcjena
                };

                previousPutovanjeViewModels.Add(putovanjeViewModel);
            }

            // Sort previous travels based on average rating
            switch (sortOrder)
            {
                case "highest":
                    previousPutovanjeViewModels = previousPutovanjeViewModels.OrderByDescending(p => p.AverageOcjena).ToList();
                    break;
                case "lowest":
                    previousPutovanjeViewModels = previousPutovanjeViewModels.OrderBy(p => p.AverageOcjena).ToList();
                    break;
            }

            var model = new PutovanjeIndexViewModel
            {
                CurrentPutovanja = currentPutovanjeViewModels,
                PreviousPutovanja = previousPutovanjeViewModels
            };

            return View(model);
        }



        [HttpGet]
        public IActionResult Reset()
        {
            return RedirectToAction(nameof(Index));
        }

        // GET: Putovanje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var putovanje = await _context.Putovanje
                .FirstOrDefaultAsync(m => m.IdPutovanje == id);

            if (putovanje == null)
            {
                return NotFound();
            }

            var stanicaPolazak = await _context.StanicaPolazak.FindAsync(putovanje.MjestoPolaska);
            var stanicaDolazak = await _context.StanicaDolazak.FindAsync(putovanje.MjestoDolaska);

            var viewModel = new PutovanjeViewModel
            {
                IdPutovanje = putovanje.IdPutovanje,
                Voz = putovanje.IdVoz,
                StanicaPolazakNaziv = stanicaPolazak?.Naziv,
                StanicaDolazakNaziv = stanicaDolazak?.Naziv,
                StanicaPolazakId=putovanje.MjestoPolaska,
                Cijena = putovanje.Cijena,
                VrijemePolaska = putovanje.VrijemePolaska,
                VrijemeDolaska = putovanje.VrijemeDolaska,
                DatumPolaska = putovanje.DatumPolaska,
                BrojMjesta = putovanje.BrojMjesta
            };

            return View(viewModel);
        }

        private bool PutovanjeExists(int id)
        {
            return _context.Putovanje.Any(e => e.IdPutovanje == id);
        }
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public IActionResult Create()
        {
            ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");
            ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv");
            ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv");
            return View(new PutovanjeViewModel());
        }

        // POST: Putovanje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Create(PutovanjeViewModel viewModel)
        {
            // Log received values
            _logger.LogInformation($"Received StanicaPolazakNaziv: {viewModel.StanicaPolazakNaziv}");
            _logger.LogInformation($"Received StanicaDolazakNaziv: {viewModel.StanicaDolazakNaziv}");
            _logger.LogInformation($"Received Voz: {viewModel.Voz}");


            if (ModelState.IsValid)
            {
                var stanicaPolazak = _context.StanicaPolazak.FirstOrDefault(s => s.IdStanicaPolazak == int.Parse(viewModel.StanicaPolazakNaziv));
                var stanicaDolazak = _context.StanicaDolazak.FirstOrDefault(s => s.IdStanicaDolazak == int.Parse(viewModel.StanicaDolazakNaziv));
                var voz = _context.Voz.FirstOrDefault(v => v.IdVoz == viewModel.Voz);

                if (stanicaPolazak == null || stanicaDolazak == null || voz == null || stanicaPolazak.IdStanicaPolazak==stanicaDolazak.IdStanicaDolazak)
                {
                    ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");

                    ModelState.AddModelError("", "Greska u odabiru stanica.");
                    ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);
                    ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
                    return View(viewModel);
                }

                if (viewModel.VrijemePolaska==viewModel.VrijemeDolaska)
                {
                    ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");

                    ModelState.AddModelError("", "Vrijeme polaska i vrijeme dolaska neispravni.");
                    ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);
                    ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
                    return View(viewModel);
                }

                var putovanje = new Putovanje
                {
                    IdVoz = voz.IdVoz,
                    MjestoPolaska = stanicaPolazak.IdStanicaPolazak,
                    MjestoDolaska = stanicaDolazak.IdStanicaDolazak,
                    Cijena = viewModel.Cijena,
                    VrijemePolaska = viewModel.VrijemePolaska,
                    VrijemeDolaska = viewModel.VrijemeDolaska,
                    DatumPolaska = viewModel.DatumPolaska,
                    BrojMjesta = voz.Kapacitet
                };

                _context.Add(putovanje);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "StanicaPutovanje", new { putovanjeId = putovanje.IdPutovanje, mjestoPolaska = putovanje.MjestoPolaska });
                //return RedirectToAction(nameof(Index));
            }

            ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);
            ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
            return View(viewModel);
        }



        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var putovanje = await _context.Putovanje.FirstOrDefaultAsync(p => p.IdPutovanje == id);
            if (putovanje == null)
            {
                return NotFound();
            }

            var viewModel = new PutovanjeViewModel
            {
                IdPutovanje = putovanje.IdPutovanje,
                Voz = putovanje.IdVoz,
                StanicaPolazakNaziv = _context.StanicaPolazak
                    .Where(s => s.IdStanicaPolazak == putovanje.MjestoPolaska)
                    .Select(s => s.Naziv)
                    .FirstOrDefault(),
                StanicaDolazakNaziv = _context.StanicaDolazak
                    .Where(s => s.IdStanicaDolazak == putovanje.MjestoDolaska)
                    .Select(s => s.Naziv)
                    .FirstOrDefault(),
                StanicaPolazakId=putovanje.MjestoPolaska,
                Cijena = putovanje.Cijena,
                VrijemePolaska = putovanje.VrijemePolaska,
                VrijemeDolaska = putovanje.VrijemeDolaska,
                DatumPolaska = putovanje.DatumPolaska,
                BrojMjesta = putovanje.BrojMjesta
            };
            ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");
            ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
            ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);

            return View(viewModel);
        }

        [Authorize(Roles = "Administrator, Zaposlenik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PutovanjeViewModel viewModel)
        {
            if (id != viewModel.IdPutovanje)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var putovanjeF = await _context.Putovanje.FindAsync(id); // Find and retrieve the entity by primary key
                    if (putovanjeF == null)
                    {
                        return NotFound();
                    }
                    var stanicaPolazak = _context.StanicaPolazak
                        .FirstOrDefault(s => s.IdStanicaPolazak == int.Parse(viewModel.StanicaPolazakNaziv));
                    var stanicaDolazak = _context.StanicaDolazak
                        .FirstOrDefault(s => s.IdStanicaDolazak == int.Parse(viewModel.StanicaDolazakNaziv));
                    var voz = _context.Voz.FirstOrDefault(v => v.IdVoz == viewModel.Voz);

                    if (stanicaPolazak == null || stanicaDolazak == null || voz == null || stanicaPolazak.IdStanicaPolazak == stanicaDolazak.IdStanicaDolazak)
                    {
                        ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");

                        ModelState.AddModelError("", "Greska u odabiru stanica.");
                        ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);
                        ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
                        return View(viewModel);
                    }

                    if (viewModel.VrijemePolaska == viewModel.VrijemeDolaska)
                    {
                        ViewData["Voz"] = new SelectList(_context.Voz, "IdVoz", "IdVoz");

                        ModelState.AddModelError("", "Vrijeme polaska i vrijeme dolaska neispravni.");
                        ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);
                        ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
                        return View(viewModel);
                    }

                    putovanjeF.IdVoz = viewModel.Voz;
                    putovanjeF.MjestoPolaska = stanicaPolazak.IdStanicaPolazak;
                    putovanjeF.MjestoDolaska = stanicaDolazak.IdStanicaDolazak;
                    putovanjeF.Cijena = viewModel.Cijena;
                    putovanjeF.VrijemePolaska = viewModel.VrijemePolaska;
                    putovanjeF.VrijemeDolaska = viewModel.VrijemeDolaska;
                    putovanjeF.DatumPolaska = viewModel.DatumPolaska;
                    putovanjeF.BrojMjesta = viewModel.BrojMjesta;

                    _context.Update(putovanjeF);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PutovanjeExists(viewModel.IdPutovanje))
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

            ViewData["MjestoDolaska"] = new SelectList(_context.StanicaDolazak, "IdStanicaDolazak", "Naziv", viewModel.StanicaDolazakNaziv);
            ViewData["MjestoPolaska"] = new SelectList(_context.StanicaPolazak, "IdStanicaPolazak", "Naziv", viewModel.StanicaPolazakNaziv);

            return View(viewModel);
        }




        [Authorize(Roles = "Administrator, Zaposlenik")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var putovanje = await _context.Putovanje
                .FirstOrDefaultAsync(p => p.IdPutovanje == id);

            if (putovanje == null)
            {
                return NotFound();
            }

            var viewModel = new PutovanjeViewModel
            {
                IdPutovanje = putovanje.IdPutovanje,
                Voz = putovanje.IdVoz,
                StanicaPolazakNaziv = _context.StanicaPolazak
                    .Where(s => s.IdStanicaPolazak == putovanje.MjestoPolaska)
                    .Select(s => s.Naziv)
                    .FirstOrDefault(),
                StanicaDolazakNaziv = _context.StanicaDolazak
                    .Where(s => s.IdStanicaDolazak == putovanje.MjestoDolaska)
                    .Select(s => s.Naziv)
                    .FirstOrDefault(),
                Cijena = putovanje.Cijena,
                VrijemePolaska = putovanje.VrijemePolaska,
                VrijemeDolaska = putovanje.VrijemeDolaska,
                DatumPolaska = putovanje.DatumPolaska,
                BrojMjesta = putovanje.BrojMjesta
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Administrator, Zaposlenik")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var putovanje = await _context.Putovanje.FindAsync(id);
            if (putovanje != null)
            {
                _context.Putovanje.Remove(putovanje);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Statistika(int putovanjeId)
        {
            _logger.LogInformation($"Received StanicaPolazakNaziv: {putovanjeId}");

            var putovanje = await _context.Putovanje
                .FirstOrDefaultAsync(p => p.IdPutovanje == putovanjeId);

            var voz = await _context.Voz
                .FirstOrDefaultAsync(p => p.IdVoz== putovanje.IdVoz);
            _logger.LogInformation($"Received StanicaPolazakNaziv: {putovanje.IdVoz}");
            _logger.LogInformation($"Received StanicaPolazakNaziv: {voz.IdVoz}");

            if (putovanje == null)
            {
                return NotFound();
            }

            // Fetch all recenzije for this putovanje
            var recenzije = await _context.Recenzija
                .Where(r => r.PutovanjeId == putovanjeId)
                .ToListAsync();

            // Calculate average rating
            double averageRating = recenzije.Any() ? recenzije.Average(r => r.Ocjena) : 0;

            // Get top 3 recenzije by rating
            var topRecenzije = recenzije
                .OrderByDescending(r => r.Ocjena)
                .Take(3)
                .Select(r => new Recenzija { Ocjena = r.Ocjena, Komentar = r.Komentar })
                .ToList();
            var popunjenost = ((double)(voz.Kapacitet- putovanje.BrojMjesta) / voz.Kapacitet) * 100;
            popunjenost=Math.Round(popunjenost, 4);
            _logger.LogInformation($"Popunjenost StanicaPolazakNaziv: {popunjenost}");

            var viewModel = new PutovanjeStatistikaViewModel
            {
                AverageRating = averageRating,
                KapacitetVoz = voz.Kapacitet,
                BrojMjesta = putovanje.BrojMjesta,
                TopRecenzije = topRecenzije,
                popunjenost=popunjenost
            };

            return View(viewModel);
        }


    }
}
