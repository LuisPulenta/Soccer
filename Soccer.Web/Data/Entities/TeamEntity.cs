using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class TeamEntity
    {
        public int Id { get; set; }

        [Display(Name = "Equipo")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }

        [Display(Name = "Iniciales")]
        [MaxLength(3, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El campo {0} debe tener {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Initials { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        [Display(Name = "Liga")]
        public LeagueEntity League { get; set; }

        public ICollection<GroupDetailEntity> GroupDetails { get; set; }

        public ICollection<Player> Fans { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "noimage"//null
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";

    }
}