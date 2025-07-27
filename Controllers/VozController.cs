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

    public class VozController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VozController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Voz
        public async Task<IActionResult> Index()
        {
            return View(await _context.Voz.ToListAsync());
        }

        // GET: Voz/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voz = await _context.Voz
                .FirstOrDefaultAsync(m => m.IdVoz == id);
            if (voz == null)
            {
                return NotFound();
            }

            return View(voz);
        }

        // GET: Voz/Create
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Voz/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVoz,Vrsta,Kapacitet")] Voz voz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voz);
        }

        // GET: Voz/Edit/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voz = await _context.Voz.FindAsync(id);
            if (voz == null)
            {
                return NotFound();
            }
            return View(voz);
        }

        // POST: Voz/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVoz,Vrsta,Kapacitet")] Voz voz)
        {
            if (id != voz.IdVoz)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VozExists(voz.IdVoz))
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
            return View(voz);
        }

        // GET: Voz/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voz = await _context.Voz
                .FirstOrDefaultAsync(m => m.IdVoz == id);
            if (voz == null)
            {
                return NotFound();
            }

            return View(voz);
        }

        // POST: Voz/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voz = await _context.Voz.FindAsync(id);
            if (voz != null)
            {
                _context.Voz.Remove(voz);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VozExists(int id)
        {
            return _context.Voz.Any(e => e.IdVoz == id);
        }
    }
}
