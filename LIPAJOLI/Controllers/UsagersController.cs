using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LIPAJOLI.Data;
using LIPAJOLI.Models;

namespace LIPAJOLI.Controllers
{
    public class UsagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Usagers
        public async Task<IActionResult> Index(string? recherche)
        {
            IQueryable<Usager> usagers = _context.Usagers;

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                recherche = recherche.Trim();

                usagers = usagers.Where(u =>
                    u.Nom.Contains(recherche) ||
                    u.Prenom.Contains(recherche));
            }

            usagers = usagers
                .OrderBy(u => u.Nom)
                .ThenBy(u => u.Prenom);

            return View(await usagers.ToListAsync());
        }

        // GET: Usagers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers
                .FirstOrDefaultAsync(m => m.NoAbonne == id);
            if (usager == null)
            {
                return NotFound();
            }

            return View(usager);
        }

        // GET: Usagers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoAbonne,Nom,Prenom,Statut,Defaillance,Email")] Usager usager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usager);
        }

        // GET: Usagers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers.FindAsync(id);
            if (usager == null)
            {
                return NotFound();
            }
            return View(usager);
        }

        // POST: Usagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NoAbonne,Nom,Prenom,Statut,Defaillance,Email")] Usager usager)
        {
            if (id != usager.NoAbonne)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsagerExists(usager.NoAbonne))
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
            return View(usager);
        }

        // GET: Usagers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers
                .FirstOrDefaultAsync(m => m.NoAbonne == id);
            if (usager == null)
            {
                return NotFound();
            }

            return View(usager);
        }

        // POST: Usagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usager = await _context.Usagers.FindAsync(id);
            if (usager != null)
            {
                _context.Usagers.Remove(usager);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsagerExists(string id)
        {
            return _context.Usagers.Any(e => e.NoAbonne == id);
        }
    }
}
