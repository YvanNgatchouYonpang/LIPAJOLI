using System.ComponentModel.DataAnnotations;

namespace LIPAJOLI.Models
{
    public class Exemplaire
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Etat { get; set; } = "Disponible";

        public string CodeLivre { get; set; }

        public Livre? Livre { get; set; }

        public ICollection<Emprunt> Emprunts { get; set; }
            = new List<Emprunt>();
    }
}
