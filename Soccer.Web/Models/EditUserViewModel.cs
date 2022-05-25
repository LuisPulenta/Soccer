using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string LastName { get; set; }

        
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Apodo")]
        public string NickName { get; set; }

        [Display(Name = "Foto")]
        public string Picture { get; set; }

        [Display(Name = "Foto")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Hincha de")]
        public int FavoriteTeamId { get; set; }

        [Display(Name = "Puntos")]
        public int Points { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Equipo")]
        public int TeamId { get; set; }

        [Display(Name = "Liga")]
        public int LeagueId { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }

        public IEnumerable<SelectListItem> Leagues { get; set; }

        [Display(Name = "Tipo Usuario")]
        public int UserTypeId { get; set; }

        public IEnumerable<SelectListItem> UserTypes { get; set; }

        public IEnumerable<SelectListItem> Sexs { get; set; }

        public string SexId { get; set; }
    }
}
