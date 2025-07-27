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

    public class GradController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Grad
        public async Task<IActionResult> Index()
        {
            return View(await _context.Grad.ToListAsync());
        }

        // GET: Grad/Details/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grad = await _context.Grad
                .FirstOrDefaultAsync(m => m.idGrad == id);
            if (grad == null)
            {
                return NotFound();
            }

            return View(grad);
        }

        // GET: Grad/Create
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Grad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idGrad,naziv")] Grad grad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grad);
        }

        // GET: Grad/Edit/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grad = await _context.Grad.FindAsync(id);
            if (grad == null)
            {
                return NotFound();
            }
            return View(grad);
        }

        // POST: Grad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idGrad,naziv")] Grad grad)
        {
            if (id != grad.idGrad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradExists(grad.idGrad))
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
            return View(grad);
        }

        // GET: Grad/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grad = await _context.Grad
                .FirstOrDefaultAsync(m => m.idGrad == id);
            if (grad == null)
            {
                return NotFound();
            }

            return View(grad);
        }

        // POST: Grad/Delete/5
        [Authorize(Roles = "Administrator, Zaposlenik")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grad = await _context.Grad.FindAsync(id);
            if (grad != null)
            {
                _context.Grad.Remove(grad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradExists(int id)
        {
            return _context.Grad.Any(e => e.idGrad == id);
        }
    }
}
