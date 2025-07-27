using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailConnect.Data;
using RailConnect.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailConnect.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString, string roleFilter)
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault(); // Assuming a user has only one role for simplicity

                userViewModels.Add(new UserViewModel
                {
                    Slika = user.Slika,
                    Id = user.Id,
                    Ime = user.Ime,
                    Prezime = user.Prezime,
                    Email = user.Email,
                    Role = role
                });
            }

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                userViewModels = userViewModels.Where(u => u.Ime.Contains(searchString, System.StringComparison.OrdinalIgnoreCase)
                                                        || u.Prezime.Contains(searchString, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by role
            if (!string.IsNullOrEmpty(roleFilter))
            {
                userViewModels = userViewModels.Where(u => u.Role == roleFilter).ToList();
            }

            // Sort users by role
            userViewModels = userViewModels.OrderBy(u => u.Role == "Administrator" ? 1
                                           : u.Role == "Zaposlenik" ? 2
                                           : 3).ToList();

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentRole"] = roleFilter;

            // Pass the list of roles to the view
            var rolesList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Roles" },
                new SelectListItem { Value = "Administrator", Text = "Administrator" },
                new SelectListItem { Value = "Zaposlenik", Text = "Zaposlenik" },
                new SelectListItem { Value = "Korisnik", Text = "Korisnik" }
            };

            ViewBag.RolesList = rolesList;

            return View(userViewModels);
        }



        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                Ime = user.Ime,
                Prezime = user.Prezime,
                Email = user.Email,
                Slika = user.Slika,
                Role = userRoles.FirstOrDefault()
            };

            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Ime = user.Ime,
                Prezime = user.Prezime,
                Email = user.Email,
                Slika = user.Slika,
                Role = userRoles.FirstOrDefault()
            };

            ViewData["RolesList"] = new SelectList(allRoles);

            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Ime,Prezime,Email,Slika,Role")] UserEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Ime = model.Ime;
                user.Prezime = model.Prezime;
                user.Email = model.Email;
                user.Slika = model.Slika;

                var userRoles = await _userManager.GetRolesAsync(user);
                var selectedRole = model.Role;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(selectedRole))
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                        await _userManager.AddToRoleAsync(user, selectedRole);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewData["RolesList"] = new SelectList(allRoles);

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                Ime = user.Ime,
                Prezime = user.Prezime,
                Email = user.Email,
                Slika = user.Slika,
                Role = userRoles.FirstOrDefault()
            };

            return View(model);
        }

        // POST: Users/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
