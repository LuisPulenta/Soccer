using Microsoft.AspNetCore.Mvc.Rendering;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class MatchViewModel : MatchEntity
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Local")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Equipo.")]
        public int LocalId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Visitante")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un Equipo.")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Fecha")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe elegir una Fecha.")]
        public int DateNameId { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }

        public IEnumerable<SelectListItem> DateNames { get; set; }
    }
}