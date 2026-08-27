using LIPAJOLI.Models;
using Microsoft.EntityFrameworkCore;

namespace LIPAJOLI.Data
{
    public class DbInitializer
    {
            public static void Initialize(
                ApplicationDbContext context,
                IConfiguration configuration)
            {

            context.Database.EnsureCreated();

            // Si des livres existent déjà, on ne réinsère pas les données.
            if (context.Livres.Any())
                {
                    return;
                }

                // Récupération de la configuration
                int nombreJoursEmprunt =
                    configuration.GetValue<int>(
                        "Bibliotheque:NombreJoursEmprunt");

                var categories =
                    configuration.GetSection(
                        "Bibliotheque:Categories")
                    .Get<string[]>() ?? Array.Empty<string>();

                var auteurs =
                    configuration.GetSection(
                        "Bibliotheque:Auteurs")
                    .Get<string[]>() ?? Array.Empty<string>();

                // Vérification de la configuration
                if (categories.Length < 3 || auteurs.Length < 3)
                {
                    throw new InvalidOperationException(
                        "La configuration de la bibliothèque est incomplète.");
                }

                var usagers = new List<Usager>
            {
                new Usager
                {
                    NoAbonne = "U001",
                    Nom = "Tremblay",
                    Prenom = "Jean",
                    Statut = Statut.Enseignant,
                    Defaillance = 0,
                    Email = "jean.tremblay@lipajoli.ca"
                },

                new Usager
                {
                    NoAbonne = "U002",
                    Nom = "Gagnon",
                    Prenom = "Marie",
                    Statut = Statut.Etudiant,
                    Defaillance = 0,
                    Email = "marie.gagnon@lipajoli.ca"
                },

                new Usager
                {
                    NoAbonne = "U003",
                    Nom = "Roy",
                    Prenom = "Thomas",
                    Statut = Statut.Etudiant,
                    Defaillance = 1,
                    Email = "thomas.roy@lipajoli.ca"
                }
            };

                context.Usagers.AddRange(usagers);



            var livres = new List<Livre>
            {
                new Livre
                {
                    
                    Code = "PRO001",
                    ISBN10 = "0132350882",
                    ISBN13 = "9780132350884",
                    Titre = "Clean Code",
                    Auteurs = auteurs[0],
                    Categorie = categories[0],
                    Quantite = 3,
                    Prix = 49.99m
                },

                new Livre
                {
                    
                    Code = "PRO002",
                    ISBN10 = "0134757599",
                    ISBN13 = "9780134757599",
                    Titre = "Refactoring",
                    Auteurs = auteurs[1],
                    Categorie = categories[0],
                    Quantite = 2,
                    Prix = 59.99m
                },

                new Livre
                {
                    
                    Code = "RES001",
                    ISBN10 = "0132126958",
                    ISBN13 = "9780132126953",
                    Titre = "Computer Networks",
                    Auteurs = auteurs[2],
                    Categorie = categories[1],
                    Quantite = 4,
                    Prix = 69.99m
                },

                new Livre
                {
                    
                    Code = "BAS001",
                    ISBN10 = "0073523321",
                    ISBN13 = "9780073523323",
                    Titre = "Database System Concepts",
                    Auteurs = auteurs[3],
                    Categorie = categories[2],
                    Quantite = 2,
                    Prix = 79.99m
                },

                new Livre
                {
                    
                    Code = "WEB001",
                    ISBN10 = "1718500452",
                    ISBN13 = "9781718500457",
                    Titre = "Python Crash Course",
                    Auteurs = auteurs[4],
                    Categorie = categories[3],
                    Quantite = 3,
                    Prix = 44.99m
                }
            };

                context.Livres.AddRange(livres);

                context.SaveChanges();

                

                var livre1 = livres[0];
                var livre2 = livres[1];
                var usager1 = usagers[0];
                var usager2 = usagers[1];

                DateTime aujourdHui = DateTime.Today;

                var emprunts = new List<Emprunt>
            {
                // Emprunt en cours
                new Emprunt
                {
                    DateEmprunt = aujourdHui.AddDays(-5),
                    DateLimiteRetour =
                        aujourdHui.AddDays(-5 + nombreJoursEmprunt),
                    DateRetour = null,
                    LivreId = livre1.Id,
                    UsagerNoAbonne = usager1.NoAbonne
                },

                // Emprunt retourné
                new Emprunt
                {
                    DateEmprunt = aujourdHui.AddDays(-20),
                    DateLimiteRetour =
                        aujourdHui.AddDays(-20 + nombreJoursEmprunt),
                    DateRetour = aujourdHui.AddDays(-10),
                    LivreId = livre2.Id,
                    UsagerNoAbonne = usager2.NoAbonne
                },

                // Deuxième emprunt en cours
                new Emprunt
                {
                    DateEmprunt = aujourdHui.AddDays(-2),
                    DateLimiteRetour =
                        aujourdHui.AddDays(-2 + nombreJoursEmprunt),
                    DateRetour = null,
                    LivreId = livre2.Id,
                    UsagerNoAbonne = usager2.NoAbonne
                }
            };

                context.Emprunts.AddRange(emprunts);

                context.SaveChanges();
            }
    }
    
}
