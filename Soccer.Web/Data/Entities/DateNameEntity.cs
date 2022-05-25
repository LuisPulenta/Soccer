using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class DateNameEntity
    {
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }

        public TournamentEntity Tournament { get; set; }

        public ICollection<MatchEntity> Matches { get; set; }
    }
}