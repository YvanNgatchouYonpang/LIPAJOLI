using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIPAJOLI.Models
{
    public class Emprunt
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateEmprunt { get; set; }

        [Required]
        public DateTime DateLimiteRetour { get; set; }

        public DateTime? DateRetour { get; set; }

        [Required]
        public string LivreCode { get; set; } 

        [ForeignKey(nameof(LivreCode))]
        public Livre? Livre { get; set; }

        [Required]
        public string UsagerNoAbonne { get; set; } 

        [ForeignKey(nameof(UsagerNoAbonne))]
        public Usager? Usager { get; set; }

    }
}
