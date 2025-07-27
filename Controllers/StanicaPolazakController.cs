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
    [Authorize(Roles = "Administrator, Zaposlenik")]

    public class StanicaPolazakController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PutovanjeController> _logger;


        public StanicaPolazakController(ApplicationDbContext context, ILogger<PutovanjeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: StanicaPolazak
        public async Task<IActionResult> Index(int? gradId)
        {
            var gradDict = await _context.Grad.ToDictionaryAsync(g => g.idGrad, g => g.naziv);

            var query = _context.StanicaPolazak.AsQueryable();

            if (gradId.HasValue)
            {
                query = query.Where(sp => sp.IdGrad == gradId.Value);
            }

            var viewModels = await query.Select(sp => new StanicaPolazakViewModel
            {
                IdStanicaPolazak = sp.IdStanicaPolazak,
                Naziv = sp.Naziv,
                IdGrad = sp.IdGrad,
                GradNaziv = gradDict.ContainsKey(sp.IdGrad) ? gradDict[sp.IdGrad] : "Unknown"
            }).ToListAsync();
            ViewBag.gradId = gradId;

            return View(viewModels);
        }


        // GET: StanicaPolazak/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gradDict = await _context.Grad.ToDictionaryAsync(g => g.idGrad, g => g.naziv);

            var stanicaPolazak = await _context.StanicaPolazak
                .FirstOrDefaultAsync(m => m.IdStanicaPolazak == id);
            if (stanicaPolazak == null)
            {
                return NotFound();
            }

            var viewModel = new StanicaPolazakViewModel
            {
                IdStanicaPolazak = stanicaPolazak.IdStanicaPolazak,
                Naziv = stanicaPolazak.Naziv,
                IdGrad = stanicaPolazak.IdGrad,
                GradNaziv = gradDict.ContainsKey(stanicaPolazak.IdGrad) ? gradDict[stanicaPolazak.IdGrad] : "Unknown"
            };
            ViewBag.gradId = stanicaPolazak.IdGrad;

            return View(viewModel);
        }

        // GET: StanicaPolazak/Create
        public IActionResult Create(int? gradId)
        {
            if (gradId == null)
            {
                return NotFound();
            }

            ViewBag.GradId = gradId;
            return View();
        }


        // POST: StanicaPolazak/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStanicaPolazak,Naziv,IdGrad")] StanicaPolazakViewModel viewModel, int gradId)
        {
            if (ModelState.IsValid)
            {
                var stanicaPolazak = new StanicaPolazak
                {
                    IdStanicaPolazak = viewModel.IdStanicaPolazak,
                    Naziv = viewModel.Naziv,
                    IdGrad = gradId
                };

                _context.Add(stanicaPolazak);

                var stanicaDolazak = new StanicaDolazak
                {
                    IdStanicaDolazak = viewModel.IdStanicaPolazak, // Assuming IdStanicaPolazak is used as the same Id for StanicaDolazak
                    Naziv = viewModel.Naziv,
                    IdGrad = gradId
                };

                _context.Add(stanicaDolazak);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { gradId = gradId });
            }
            return View(viewModel);
        }



        // GET: StanicaPolazak/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gradDict = await _context.Grad.ToDictionaryAsync(g => g.idGrad, g => g.naziv);

            var stanicaPolazak = await _context.StanicaPolazak.FindAsync(id);
            if (stanicaPolazak == null)
            {
                return NotFound();
            }

            var viewModel = new StanicaPolazakViewModel
            {
                IdStanicaPolazak = stanicaPolazak.IdStanicaPolazak,
                Naziv = stanicaPolazak.Naziv,
                IdGrad = stanicaPolazak.IdGrad,
                GradNaziv = gradDict.ContainsKey(stanicaPolazak.IdGrad) ? gradDict[stanicaPolazak.IdGrad] : "Unknown"
            };
            ViewBag.GradId = stanicaPolazak.IdGrad;

            return View(viewModel);
        }


        // POST: StanicaPolazak/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStanicaPolazak,Naziv,IdGrad")] StanicaPolazakViewModel viewModel, int gradId)
        {
            if (id != viewModel.IdStanicaPolazak)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var stanicaPolazak = new StanicaPolazak
                    {
                        IdStanicaPolazak = viewModel.IdStanicaPolazak,
                        Naziv = viewModel.Naziv,
                        IdGrad = gradId
                    };

                    var stanicaDolazak = new StanicaDolazak
                    {
                        IdStanicaDolazak = viewModel.IdStanicaPolazak, // Assuming IdStanicaPolazak is used as the same Id for StanicaDolazak
                        Naziv = viewModel.Naziv,
                        IdGrad = gradId
                    };

                    _context.Update(stanicaDolazak);

                    _context.Update(stanicaPolazak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StanicaPolazakExists(viewModel.IdStanicaPolazak))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { gradId = gradId });
            }
            return View(viewModel);
        }


        // GET: StanicaPolazak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stanicaPolazak = await _context.StanicaPolazak
                .FirstOrDefaultAsync(m => m.IdStanicaPolazak == id);
            if (stanicaPolazak == null)
            {
                return NotFound();
            }

            var gradDict = await _context.Grad.ToDictionaryAsync(g => g.idGrad, g => g.naziv);

            var viewModel = new StanicaPolazakViewModel
            {
                IdStanicaPolazak = stanicaPolazak.IdStanicaPolazak,
                Naziv = stanicaPolazak.Naziv,
                IdGrad = stanicaPolazak.IdGrad,
                GradNaziv = gradDict.ContainsKey(stanicaPolazak.IdGrad) ? gradDict[stanicaPolazak.IdGrad] : "Unknown"
            };
            ViewBag.gradId = stanicaPolazak.IdGrad;

            return View(viewModel);
        }

        // POST: StanicaPolazak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stanicaPolazak = await _context.StanicaPolazak.FindAsync(id);
            var stanicaDolazak = await _context.StanicaDolazak.FindAsync(id);

            if (stanicaPolazak != null)
            {
                _context.StanicaPolazak.Remove(stanicaPolazak);
                _context.StanicaDolazak.Remove(stanicaDolazak);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { gradId = stanicaPolazak.IdGrad });
        }

        private bool StanicaPolazakExists(int id)
        {
            return _context.StanicaPolazak.Any(e => e.IdStanicaPolazak == id);
        }
    }
}
