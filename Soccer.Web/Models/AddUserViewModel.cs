﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class AddUserViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [EmailAddress]
        public string Username { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Document { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Address { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime BornDate { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(6, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Apodo")]
        public string NickName { get; set; }

        [Display(Name = "Foto")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Hincha de")]
        public int FavoriteTeamId { get; set; }

        [Display(Name = "Puntos")]
        public int Points { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string PhoneNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Latitude { get; set; }

        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Longitude { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        public string Password { get; set; }

        [Display(Name = "Confirmación de Password")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

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
    }
}