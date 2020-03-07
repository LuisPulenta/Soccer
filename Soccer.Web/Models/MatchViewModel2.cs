using Microsoft.AspNetCore.Mvc.Rendering;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class MatchViewModel2 : MatchEntity
    {
        public int DateNameId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Local")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Equipo.")]
        public int LocalId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Visitante")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Equipo.")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Grupo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Grupo.")]
        public int GroupId { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }
    }
}