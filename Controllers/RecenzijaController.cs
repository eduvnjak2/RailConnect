using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailConnect.Data;
using RailConnect.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RailConnect.Controllers
{
    public class RecenzijaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PutovanjeController> _logger;


        public RecenzijaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PutovanjeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Recenzija
        public async Task<IActionResult> Index(int? putovanjeId)
        {
            if (putovanjeId == null)
            {
                return NotFound();
            }

            var recenzijas = await _context.Recenzija
                .Where(r => r.PutovanjeId == putovanjeId)
                .Select(r => new RecenzijaViewModel
                {
                    IdRecencija = r.IdRecencija,
                    PutnikId = r.PutnikId,
                    PutovanjeId = r.PutovanjeId,
                    Ocjena = r.Ocjena,
                    Komentar = r.Komentar,
                    Ime = _context.Users.Where(u => u.Id == r.PutnikId).Select(u => u.Ime).FirstOrDefault(),
                    Prezime = _context.Users.Where(u => u.Id == r.PutnikId).Select(u => u.Prezime).FirstOrDefault()
                })
                .ToListAsync();

            ViewBag.PutovanjeId = putovanjeId;
            return View(recenzijas);
        }

        // GET: Recenzija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _context.Recenzija
                .FirstOrDefaultAsync(m => m.IdRecencija == id);

            if (recenzija == null)
            {
                return NotFound();
            }

            var viewModel = new RecenzijaViewModel
            {
                IdRecencija = recenzija.IdRecencija,
                PutnikId = recenzija.PutnikId,
                PutovanjeId = recenzija.PutovanjeId,
                Ocjena = recenzija.Ocjena,
                Komentar = recenzija.Komentar,
                Ime = _context.Users.Where(u => u.Id == recenzija.PutnikId).Select(u => u.Ime).FirstOrDefault(),
                Prezime = _context.Users.Where(u => u.Id == recenzija.PutnikId).Select(u => u.Prezime).FirstOrDefault()
            };
            ViewBag.PutovanjeId = recenzija.PutovanjeId;

            return View(viewModel);
        }

        // GET: Recenzija/Create
        public IActionResult Create(int putovanjeId)
        {
            var viewModel = new RecenzijaViewModel
            {
                PutovanjeId = putovanjeId
            };
            ViewBag.PutovanjeId = putovanjeId;
            return View(viewModel);
        }

        // POST: Recenzija/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecenzijaViewModel recenzijaViewModel)
        {
            if (ModelState.IsValid)
            {
                var recenzija = new Recenzija
                {
                    PutnikId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    PutovanjeId = recenzijaViewModel.PutovanjeId,
                    Ocjena = recenzijaViewModel.Ocjena,
                    Komentar = recenzijaViewModel.Komentar
                };

                _context.Add(recenzija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { putovanjeId = recenzijaViewModel.PutovanjeId });
            }

            // Log the validation errors for debugging purposes
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Validation Error: {error.ErrorMessage}");
            }

            return View(recenzijaViewModel);
        }



        // GET: Recenzija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _context.Recenzija.FindAsync(id);
            if (recenzija == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (recenzija.PutnikId != user.Id)
            {
                return Forbid();
            }

            var viewModel = new RecenzijaViewModel
            {
                IdRecencija = recenzija.IdRecencija,
                PutnikId = recenzija.PutnikId,
                PutovanjeId = recenzija.PutovanjeId,
                Ocjena = recenzija.Ocjena,
                Komentar = recenzija.Komentar,
                Ime = user.Ime,
                Prezime = user.Prezime
            };
            ViewBag.PutovanjeId = recenzija.PutovanjeId;

            return View(viewModel);
        }

        // POST: Recenzija/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRecencija,PutovanjeId,Ocjena,Komentar")] RecenzijaViewModel recenzijaViewModel)
        {
            if (id != recenzijaViewModel.IdRecencija)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var recenzija = await _context.Recenzija.FindAsync(id);
            if (recenzija.PutnikId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    recenzija.PutovanjeId = recenzijaViewModel.PutovanjeId;
                    recenzija.Ocjena = recenzijaViewModel.Ocjena;
                    recenzija.Komentar = recenzijaViewModel.Komentar;

                    _context.Update(recenzija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecenzijaExists(recenzijaViewModel.IdRecencija))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { putovanjeId = recenzija.PutovanjeId });
            }
            return View(recenzijaViewModel);
        }

        // GET: Recenzija/Delete/5
       /* public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _context.Recenzija
                .FirstOrDefaultAsync(m => m.IdRecencija == id);
            if (recenzija == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (recenzija.PutnikId != user.Id)
            {
                return Forbid();
            }
            ViewBag.PutovanjeId = recenzija.PutovanjeId;

            return View(recenzija);
        }

        // POST: Recenzija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzija = await _context.Recenzija.FindAsync(id);
            if (recenzija != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (recenzija.PutnikId != user.Id)
                {
                    return Forbid();
                }

                _context.Recenzija.Remove(recenzija);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { putovanjeId = recenzija.PutovanjeId });
        }*/


        // GET: StanicaPutovanje/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _context.Recenzija
                .FirstOrDefaultAsync(m => m.IdRecencija == id);
            if (recenzija == null)
            {
                return NotFound();
            }

            ViewBag.PutovanjeId = recenzija.PutovanjeId;
            var user = await _userManager.GetUserAsync(User);
            if (recenzija.PutnikId != user.Id)
            {
                return Forbid();
            }

            var viewModel = new RecenzijaViewModel
            {
                IdRecencija = recenzija.IdRecencija,
                PutovanjeId = recenzija.PutovanjeId,
                Ocjena = recenzija.Ocjena,
                Komentar = recenzija.Komentar,
                Ime = "", // Include any necessary user info if needed
                Prezime = "" // Include any necessary user info if needed
            };

            return View(viewModel);
        }

        // POST: StanicaPutovanje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzija = await _context.Recenzija.FindAsync(id);

            if (recenzija != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (recenzija.PutnikId != user.Id)
                {
                    return Forbid();
                }
                _context.Recenzija.Remove(recenzija);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Recenzija", new { putovanjeId = recenzija.PutovanjeId});

        }

        private bool RecenzijaExists(int id)
        {
            return _context.Recenzija.Any(e => e.IdRecencija == id);
        }
    }
}
