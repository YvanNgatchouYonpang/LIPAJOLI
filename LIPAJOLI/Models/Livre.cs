using System.ComponentModel.DataAnnotations;

namespace LIPAJOLI.Models
{
    public class Livre
    {
        [Key]
        //[Required(ErrorMessage = "Le code du livre est obligatoire.")]
        public string? Code { get; set; } 

        [Required(ErrorMessage = "Le numéro ISBN-10 est obligatoire.")]
        [StringLength(10, MinimumLength = 10,
            ErrorMessage = "Le numéro ISBN-10 doit contenir exactement 10 caractères.")]
        public string ISBN10 { get; set; } 

        [Required(ErrorMessage = "Le numéro ISBN-13 est obligatoire.")]
        [StringLength(13, MinimumLength = 13,
            ErrorMessage = "Le numéro ISBN-13 doit contenir exactement 13 caractères.")]
        public string ISBN13 { get; set; } 

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; } 

        [Required(ErrorMessage = "Au moins un auteur est obligatoire.")]
        public string Auteurs { get; set; } 

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public string Categorie { get; set; } 

        [Range(0, int.MaxValue,
            ErrorMessage = "La quantité doit être supérieure ou égale à 0.")]
        [Required(ErrorMessage = "La quantite est obligatoire.")]
        public int Quantite { get; set; }

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Le prix doit être supérieur à 0.")]
        [Required(ErrorMessage = "Le prix est obligatoire.")]
        public decimal Prix { get; set; }

        public ICollection<Emprunt> Emprunts { get; set; }
            = new List<Emprunt>();

    }
}
