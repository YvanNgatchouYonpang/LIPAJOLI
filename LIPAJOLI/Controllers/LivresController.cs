using LIPAJOLI.Data;
using LIPAJOLI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(string? recherche, string currentFilter,int?numeroPage,
            string? tri)
        {
            ViewData["CurrentSort"] = tri;
            ViewData["ParamCodeTri"] =String.IsNullOrEmpty(tri) ? "Code_desc" : "";
            ViewData["ParamTitreTri"] = tri == "Titre" ? "Titre_desc" : "Titre";

            IQueryable<Livre> livres = _context.Livres;

            ViewData["CurrentFilter"] = recherche;

            livres= from l in _context.Livres
                    select l;

            // Recherche
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                recherche = recherche.Trim().ToLower();

                livres = livres.Where(l => l.Titre.ToLower().Contains(recherche) || l.Auteurs.ToLower().Contains(recherche) ||l.Categorie.ToLower().Contains(recherche));
            }

            // Tri
          
            if (string.IsNullOrEmpty(tri))
            {
                tri = "Code";
            }

            bool descending = false;
            if (tri.EndsWith("_desc"))
            {
                tri = tri.Substring(0, tri.Length - 5);
                descending = true;
            }

            if (descending)
            {
                livres = livres.OrderByDescending(l => EF.Property<object>(l, tri));
            }
            else
            {
                livres = livres.OrderBy(l => EF.Property<object>(l, tri));
            }

            int pageSize = 3;

            //return View(await livres.ToListAsync());
            return View(await PaginatedList<Livre>.CreateAsync(livres.AsNoTracking(),
               numeroPage ?? 1, pageSize));
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
        public async Task<IActionResult> Create([Bind("Code,ISBN10,ISBN13,Titre,Auteurs,Categorie,Quantite,Prix")] Livre livre,List<string>listeAuteurs)
        {
            if (!ModelState.IsValid)
            {
                return View(livre);
            }

           
            // Génération automatique du code
            livre.Code = GenererCodeLivre(livre.Categorie);

            livre.Auteurs = string.Join("; ", listeAuteurs);

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
        public async Task<IActionResult> Edit(string id, [Bind("Code,ISBN10,ISBN13,Titre,Auteurs,Categorie,Quantite,Prix")] Livre livre, List<string> listeAuteurs)
        {
            if (id != livre.Code)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(livre);
            }

            Livre? modificationLivre = await _context.Livres.FirstOrDefaultAsync(l => l.Code == id);

            try
            {
                if (modificationLivre.Categorie != livre.Categorie)
                {
                    modificationLivre.Code = GenererCodeLivre(livre.Categorie);
                }

                modificationLivre.ISBN10 = livre.ISBN10;
                modificationLivre.ISBN13 = livre.ISBN13;
                modificationLivre.Titre = livre.Titre;
                modificationLivre.Auteurs= string.Join("; ", listeAuteurs);
                modificationLivre.Categorie= livre.Categorie;
                modificationLivre.Quantite= livre.Quantite;
                modificationLivre.Prix= livre.Prix;

              //  _context.Update(livre);

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

    }

}
