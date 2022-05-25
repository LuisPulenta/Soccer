using Microsoft.AspNetCore.Identity;
using Soccer.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class User : IdentityUser
    {
      
        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string LastName { get; set; }


        [Display(Name = "Foto")]
        public string Picture { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(Picture)
           ? "http://keypress.serveftp.net:88/SoccerApi/Images/users/nouser.png"
           : $"http://keypress.serveftp.net:88/SoccerApi/Soccer{Picture.Substring(1)}";

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Apodo")]
        public string NickName { get; set; }

        [Display(Name = "Hincha de")]
        public TeamEntity FavoriteTeam { get; set; }

        [Display(Name = "Puntos")]
        public int Points { get; set; }

        [Display(Name = "Nombre Completo")]
        public string FullName => $"{FirstName} {LastName}";
    }
}