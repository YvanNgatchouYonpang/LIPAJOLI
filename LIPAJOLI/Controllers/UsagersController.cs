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
                .Include(u => u.Emprunts)
                    .ThenInclude(e => e.Livre)
                .FirstOrDefaultAsync(u => u.NoAbonne == id);

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
            // La défaillance est toujours initialisée à 0
            usager.Defaillance = 0;

            // Vérifier que le numéro d'abonné n'existe pas déjà
            bool numeroExiste = await _context.Usagers
                .AnyAsync(u => u.NoAbonne == usager.NoAbonne);

            if (numeroExiste)
            {
                ModelState.AddModelError(
                    nameof(usager.NoAbonne),
                    "Ce numéro d'abonné existe déjà.");
            }

            if (!ModelState.IsValid)
            {
                return View(usager);
            }

            _context.Usagers.Add(usager);

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "L'usager a été ajouté avec succès.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Usagers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers
                .FirstOrDefaultAsync(u => u.NoAbonne == id);

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

            if (!ModelState.IsValid)
            {
                return View(usager);
            }

            try
            {
                var usagerExistant = await _context.Usagers
                    .FirstOrDefaultAsync(u => u.NoAbonne == id);

                if (usagerExistant == null)
                {
                    return NotFound();
                }

                // Le numéro d'abonné ne change pas
                usagerExistant.NoAbonne = usager.NoAbonne;

                // Modification des informations autorisées
                usagerExistant.Nom = usager.Nom;
                usagerExistant.Prenom = usager.Prenom;
                usagerExistant.Statut = usager.Statut;
                usagerExistant.Email = usager.Email;

                // La défaillance n'est volontairement pas modifiée

                await _context.SaveChangesAsync();

                TempData["Succes"] =
                    "L'usager a été modifié avec succès.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool existe = await _context.Usagers
                    .AnyAsync(u => u.NoAbonne == id);

                if (!existe)
                {
                    return NotFound();
                }

                throw;
            }

        }

        // GET: Usagers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers
                .FirstOrDefaultAsync(u => u.NoAbonne == id);

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
            if (id == null)
            {
                return NotFound();
            }

            var usager = await _context.Usagers
                .Include(u => u.Emprunts)
                .FirstOrDefaultAsync(u => u.NoAbonne == id);

            if (usager == null)
            {
                return NotFound();
            }

            // Un usager ayant un historique d'emprunts
            // ne doit pas être supprimé.
            if (usager.Emprunts != null && usager.Emprunts.Any())
            {
                TempData["Erreur"] =
                    "Cet usager ne peut pas être supprimé car il possède un historique d'emprunts.";

                return RedirectToAction(nameof(Index));
            }

            _context.Usagers.Remove(usager);

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "L'usager a été supprimé avec succès.";

            return RedirectToAction(nameof(Index));
        }

        private bool UsagerExists(string id)
        {
            return _context.Usagers.Any(e => e.NoAbonne == id);
        }
    }
}
