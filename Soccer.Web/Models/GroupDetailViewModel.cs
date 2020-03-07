using Microsoft.AspNetCore.Mvc.Rendering;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class GroupDetailViewModel : GroupDetailEntity
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Equipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir una Liga.")]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Equipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Equipo.")]
        public int TeamId { get; set; }

        public IEnumerable<SelectListItem> Leagues { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }
    }
}