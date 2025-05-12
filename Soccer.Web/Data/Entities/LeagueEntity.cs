using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class LeagueEntity
    {
        public int Id { get; set; }

        [Display(Name = "Liga")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        public ICollection<TeamEntity> Teams { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
         ? "https://keypress.serveftp.net/SoccerApi/Images/leagues/noimage.png"
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}