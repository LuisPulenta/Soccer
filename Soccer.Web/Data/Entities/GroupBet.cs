using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Soccer.Web.Data.Entities
{
    public class GroupBet
    {
        public int Id { get; set; }

        [Display(Name = "Grupo")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        [Display(Name = "Administrador")]
        public Player Admin { get; set; }

        public TournamentEntity Tournament { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<GroupBetPlayer> GroupBetPlayers { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "http://keypress.serveftp.net:88/SoccerApi/Images/noimage.png"
           : $"http://keypress.serveftp.net:88/SoccerApi{LogoPath.Substring(1)}";
    }
}