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
    public class LivresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LivresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Livres
        public async Task<IActionResult> Index(string? recherche,
            string? tri)
        {
            IQueryable<Livre> livres = _context.Livres;

            // Recherche
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                recherche = recherche.Trim();

                livres = livres.Where(l => l.Titre.Contains(recherche) || l.Auteurs.Contains(recherche) ||l.Categorie.Contains(recherche));
            }

            // Tri
            livres = tri switch
            {
                "code" => livres.OrderBy(l => l.Code),

                "code_desc" => livres.OrderByDescending(l => l.Code),

                "titre" => livres.OrderBy(l => l.Titre),

                "titre_desc" => livres.OrderByDescending(l => l.Titre),

                _ => livres.OrderBy(l => l.Code)
            };

            return View(await livres.ToListAsync());
        }


        // GET: Livres/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.Include(l => l.Emprunts).FirstOrDefaultAsync(l => l.Code == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // GET: Livres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Livres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,ISBN10,ISBN13,Titre,Auteurs,Categorie,Quantite,Prix")] Livre livre)
        {
            if (!ModelState.IsValid)
            {
                return View(livre);
            }

            // Vérification ISBN
            if (!ValiderIsbn10(livre.ISBN10))
            {
                ModelState.AddModelError(
                    nameof(livre.ISBN10),
                    "Le numéro ISBN-10 est invalide.");

                return View(livre);
            }

            if (!ValiderIsbn13(livre.ISBN13))
            {
                ModelState.AddModelError(
                    nameof(livre.ISBN13),
                    "Le numéro ISBN-13 est invalide.");

                return View(livre);
            }

            // Génération automatique du code
            livre.Code = GenererCodeLivre(livre.Categorie);

            _context.Livres.Add(livre);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Livres/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FirstOrDefaultAsync(l => l.Code == id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }

        // POST: Livres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Code,ISBN10,ISBN13,Titre,Auteurs,Categorie,Quantite,Prix")] Livre livre)
        {
            if (id != livre.Code)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(livre);
            }

            if (!ValiderIsbn10(livre.ISBN10))
            {
                ModelState.AddModelError(
                    nameof(livre.ISBN10),
                    "Le numéro ISBN-10 est invalide.");

                return View(livre);
            }

            if (!ValiderIsbn13(livre.ISBN13))
            {
                ModelState.AddModelError(
                    nameof(livre.ISBN13),
                    "Le numéro ISBN-13 est invalide.");

                return View(livre);
            }

            try
            {
                _context.Update(livre);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivreExists(livre.Code))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Livres/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .FirstOrDefaultAsync(m => m.Code == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.Emprunts)
                .FirstOrDefaultAsync(l => l.Code == id);

            if (livre == null)
            {
                return NotFound();
            }

            // Vérifier si le livre possède un historique d'emprunts
            if (livre.Emprunts != null && livre.Emprunts.Any())
            {
                TempData["Erreur"] =
                    "Ce livre ne peut pas être supprimé car il possède un historique d'emprunts.";

                return RedirectToAction(nameof(Index));
            }

            _context.Livres.Remove(livre);

            await _context.SaveChangesAsync();

            TempData["Succes"] = "Le livre a été supprimé avec succès.";

            return RedirectToAction(nameof(Index));
        }

        private bool LivreExists(string code)
        {
            return _context.Livres.Any(e => e.Code == code);
        }







        private string GenererCodeLivre(string categorie)
        {
            string prefixe = categorie
                .Trim()
                .Substring(0, Math.Min(3, categorie.Trim().Length))
                .ToUpper();

            var derniersLivres = _context.Livres
                .Where(l => l.Code.StartsWith(prefixe))
                .ToList();

            int prochainNumero = 1;

            if (derniersLivres.Any())
            {
                int dernierNumero = derniersLivres
                    .Select(l =>
                    {
                        string numero = l.Code.Substring(3);

                        return int.TryParse(numero, out int valeur)
                            ? valeur
                            : 0;
                    })
                    .Max();

                prochainNumero = dernierNumero + 1;
            }

            return $"{prefixe}{prochainNumero:D3}";
        }





        private bool ValiderIsbn10(string isbn)
        {
            isbn = isbn.Replace("-", "")
                       .Replace(" ", "")
                       .ToUpper();

            if (isbn.Length != 10)
                return false;

            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn[i]))
                    return false;
            }

            if (!(char.IsDigit(isbn[9]) || isbn[9] == 'X'))
                return false;

            int somme = 0;

            for (int i = 0; i < 9; i++)
            {
                somme += (isbn[i] - '0') * (10 - i);
            }

            int dernierChiffre =
                isbn[9] == 'X' ? 10 : isbn[9] - '0';

            somme += dernierChiffre;

            return somme % 11 == 0;
        }



        private bool ValiderIsbn13(string isbn)
        {
            isbn = isbn.Replace("-", "")
                       .Replace(" ", "");

            if (isbn.Length != 13)
                return false;

            if (!isbn.All(char.IsDigit))
                return false;

            int somme = 0;

            for (int i = 0; i < 12; i++)
            {
                int chiffre = isbn[i] - '0';

                somme += i % 2 == 0
                    ? chiffre
                    : chiffre * 3;
            }

            int chiffreControle =
                (10 - (somme % 10)) % 10;

            return chiffreControle == isbn[12] - '0';
        }


    }
}
