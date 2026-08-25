using System.ComponentModel.DataAnnotations;

namespace LIPAJOLI.Models
{
    public class Usager
    {
        [Key]
        [Required(ErrorMessage = "Le numéro d'abonné est obligatoire.")]
        public string NoAbonne { get; set; } 

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100,
            ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } 

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(100,
            ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
        public string Prenom { get; set; } 

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public Statut Statut { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Le nombre de défaillances doit être supérieur ou égal à 0.")]
        public int Defaillance { get; set; } = 0;

        [Required(ErrorMessage = "L'adresse courriel est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse courriel n'est pas valide.")]
        public string Email { get; set; } 

        public ICollection<Emprunt> Emprunts { get; set; }
            = new List<Emprunt>();

    }
}
