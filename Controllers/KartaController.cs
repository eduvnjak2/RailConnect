using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailConnect.Data;
using RailConnect.Models;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;

namespace RailConnect.Controllers
{
    public class KartaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KartaController> _logger;
        private readonly IConfiguration _configuration;

        public KartaController(ApplicationDbContext context, ILogger<KartaController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: Karta
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Get the current user ID
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fetch all Karta entries associated with the current user
            var kartas = await _context.Karta
                .Where(k => k.PutnikId == currentUserId) // Filter by current user
                .ToListAsync(); // Fetch the data asynchronously

            // Create a list to collect the final KartaViewModel entries
            var model = new KartaIndexViewModel();

            // Iterate through each Karta entry
            foreach (var k in kartas)
            {
                // Fetch associated Putovanje entry
                var putovanje = await _context.Putovanje.FindAsync(k.PutovanjeId);

                // Create KartaViewModel with relevant details
                var kartaviewmodel = new KartaViewModel
                {
                    IdKarta = k.IdKarta,
                    IdStanicaPolazak = k.IdStanicaPolazak,
                    StanicaPolazakNaziv = (await _context.StanicaPolazak.FindAsync(k.IdStanicaPolazak)).Naziv,
                    IdStanicaDolazak = k.IdStanicaDolazak,
                    StanicaDolazakNaziv = (await _context.StanicaDolazak.FindAsync(k.IdStanicaDolazak)).Naziv,
                    PutnikId = k.PutnikId,
                    PutovanjeId = k.PutovanjeId,
                    DatumPutovanja=putovanje.DatumPolaska,
                    NacinPlacanja = k.NacinPlacanja
                };

                if (putovanje.DatumPolaska > DateTime.Now)
                {
                    model.ValidKartas.Add(kartaviewmodel); // Add to valid list
                }
                else
                {
                    model.PreviousKartas.Add(kartaviewmodel); // Add to previous list
                }
            }

            model.ValidKartas.Sort((x, y) => DateTime.Compare(x.DatumPutovanja, y.DatumPutovanja));
            // Sort the PreviousKartas list by DatumPutovanja from smallest to largest
            model.PreviousKartas.Sort((x, y) => DateTime.Compare(x.DatumPutovanja, y.DatumPutovanja));
            // Return the view with the ViewModel containing the filtered entries
            var user = await _context.ApplicationUser.FindAsync(currentUserId);
            ViewBag.UserEmail = user.Email;
            ViewBag.UserId = currentUserId;

            return View(model);
        }

        // GET: Karta/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta
                .Where(k => k.IdKarta == id)
                .Join(_context.StanicaPolazak, k => k.IdStanicaPolazak, sp => sp.IdStanicaPolazak, (k, sp) => new { k, sp })
                .Join(_context.StanicaDolazak, ksp => ksp.k.IdStanicaDolazak, sd => sd.IdStanicaDolazak, (ksp, sd) => new KartaViewModel
                {
                    IdKarta = ksp.k.IdKarta,
                    IdStanicaPolazak = ksp.k.IdStanicaPolazak,
                    StanicaPolazakNaziv = ksp.sp.Naziv,
                    IdStanicaDolazak = ksp.k.IdStanicaDolazak,
                    StanicaDolazakNaziv = sd.Naziv,
                    PutnikId = ksp.k.PutnikId,
                    PutovanjeId = ksp.k.PutovanjeId,
                    NacinPlacanja = ksp.k.NacinPlacanja
                })
                .FirstOrDefaultAsync();
            var putovanje = await _context.Putovanje.FindAsync(karta.PutovanjeId);
            karta.DatumPutovanja = putovanje.DatumPolaska;


            if (karta == null)
            {
                return NotFound();
            }

            return View(karta);
        }

        // GET: Karta/Create
        [Authorize]

        public IActionResult Create(int putovanjeId, int idStanicaPolazak, int idStanicaDolazak)
        {
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var stanicaPolazak = _context.StanicaPolazak.FirstOrDefault(s => s.IdStanicaPolazak == idStanicaPolazak);
            var stanicaDolazak = _context.StanicaDolazak.FirstOrDefault(s => s.IdStanicaDolazak == idStanicaDolazak);

            var kartaViewModel = new KartaViewModel
            {
                PutovanjeId = putovanjeId,
                IdStanicaPolazak = idStanicaPolazak,
                StanicaPolazakNaziv = stanicaPolazak?.Naziv,
                IdStanicaDolazak = idStanicaDolazak,
                StanicaDolazakNaziv = stanicaDolazak?.Naziv,
                PutnikId = currentUserId,
            };

            ViewData["PutovanjeId"] = putovanjeId;
            ViewData["IdStanicaPolazak"] = idStanicaPolazak;
            ViewData["IdStanicaDolazak"] = idStanicaDolazak;
            ViewData["PutnikId"] = currentUserId;

            return View(kartaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("PutovanjeId,IdStanicaPolazak,StanicaPolazakNaziv,IdStanicaDolazak,StanicaDolazakNaziv,PutnikId,NacinPlacanja")] KartaViewModel kartaViewModel)
        {
            if (ModelState.IsValid)
            {
                var karta = new Karta
                {
                    PutovanjeId = kartaViewModel.PutovanjeId,
                    IdStanicaPolazak = kartaViewModel.IdStanicaPolazak,
                    IdStanicaDolazak = kartaViewModel.IdStanicaDolazak,
                    PutnikId = kartaViewModel.PutnikId,
                    NacinPlacanja = kartaViewModel.NacinPlacanja
                };

                _context.Add(karta);

                var putovanje = await _context.Putovanje.FindAsync(karta.PutovanjeId);
                if (putovanje != null)
                {
                    putovanje.BrojMjesta--;
                    _context.Update(putovanje);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PutovanjeId"] = kartaViewModel.PutovanjeId;
            ViewData["IdStanicaPolazak"] = kartaViewModel.IdStanicaPolazak;
            ViewData["IdStanicaDolazak"] = kartaViewModel.IdStanicaDolazak;
            ViewData["PutnikId"] = kartaViewModel.PutnikId;

            return View(kartaViewModel);
        }

        // GET: Karta/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta
                .Where(k => k.IdKarta == id)
                .Join(_context.StanicaPolazak, k => k.IdStanicaPolazak, sp => sp.IdStanicaPolazak, (k, sp) => new { k, sp })
                .Join(_context.StanicaDolazak, ksp => ksp.k.IdStanicaDolazak, sd => sd.IdStanicaDolazak, (ksp, sd) => new KartaViewModel
                {
                    IdKarta = ksp.k.IdKarta,
                    IdStanicaPolazak = ksp.k.IdStanicaPolazak,
                    StanicaPolazakNaziv = ksp.sp.Naziv,
                    IdStanicaDolazak = ksp.k.IdStanicaDolazak,
                    StanicaDolazakNaziv = sd.Naziv,
                    PutnikId = ksp.k.PutnikId,
                    PutovanjeId = ksp.k.PutovanjeId,
                    NacinPlacanja = ksp.k.NacinPlacanja
                })
                .FirstOrDefaultAsync();

            if (karta == null)
            {
                return NotFound();
            }

            return View(karta);
        }

        // POST: Karta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Edit(int id, [Bind("IdKarta,IdStanicaPolazak,StanicaPolazakNaziv,IdStanicaDolazak,StanicaDolazakNaziv,PutnikId,PutovanjeId,NacinPlacanja")] KartaViewModel viewModel)
        {
            if (id != viewModel.IdKarta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var karta = await _context.Karta.FindAsync(id);
                    if (karta == null)
                    {
                        return NotFound();
                    }

                    karta.NacinPlacanja = viewModel.NacinPlacanja;

                    _context.Update(karta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KartaExists(viewModel.IdKarta))
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
            return View(viewModel);
        }

        // GET: Karta/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta
                .Where(k => k.IdKarta == id)
                .Join(_context.StanicaPolazak, k => k.IdStanicaPolazak, sp => sp.IdStanicaPolazak, (k, sp) => new { k, sp })
                .Join(_context.StanicaDolazak, ksp => ksp.k.IdStanicaDolazak, sd => sd.IdStanicaDolazak, (ksp, sd) => new KartaViewModel
                {
                    IdKarta = ksp.k.IdKarta,
                    IdStanicaPolazak = ksp.k.IdStanicaPolazak,
                    StanicaPolazakNaziv = ksp.sp.Naziv,
                    IdStanicaDolazak = ksp.k.IdStanicaDolazak,
                    StanicaDolazakNaziv = sd.Naziv,
                    PutnikId = ksp.k.PutnikId,
                    PutovanjeId = ksp.k.PutovanjeId,
                    NacinPlacanja = ksp.k.NacinPlacanja
                })
                .FirstOrDefaultAsync();

            if (karta == null)
            {
                return NotFound();
            }

            return View(karta);
        }

        // POST: Karta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var karta = await _context.Karta.FindAsync(id);
            if (karta != null)
            {
                var putovanje = await _context.Putovanje.FindAsync(karta.PutovanjeId);
                if (putovanje != null)
                {
                    putovanje.BrojMjesta++;
                    _context.Update(putovanje);
                }

                _context.Karta.Remove(karta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool KartaExists(int id)
        {
            return _context.Karta.Any(e => e.IdKarta == id);
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail(string userId)
        {
            var user = await _context.ApplicationUser.FindAsync(userId);
            _logger.LogInformation("Initiating email sending process.");
            _logger.LogInformation($"Initiating email sending process: {userId}");


            try
            {
                await SendEmailToUser(userId);
                _logger.LogInformation("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            return Ok();
        }

        public async Task SendEmailToUser(string userId)
        {

            string userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"Retrieving user with ID: {userid}");


            if (userid == null)
            {
                _logger.LogWarning("User not found.");
                return;
            }
            var user = await _context.ApplicationUser.FindAsync(userid);

            string userEmail = user.Email;
            _logger.LogInformation($"Sending email to: {userEmail}");

            using (MailMessage mail = new MailMessage())
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential("railconnect17@gmail.com", "zphf bbmn tfxb kvar"),
                    EnableSsl = true
                };

                mail.From = new MailAddress("railconnect17@gmail.com");
                mail.To.Add(userEmail);
                mail.Subject = "Potvrda kupovine karte";
                mail.Body = "Pozdrav, hvala Vam sto ste nas odabrali i dali nam povjerenje da Vas sigurno i ugodno prevezemo na Vasu destinaciju. Vas RailConnect tim.";
                try
                {
                    await SmtpServer.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email: {ex.Message}");
                    throw;
                }
            }
        }



    }
}
