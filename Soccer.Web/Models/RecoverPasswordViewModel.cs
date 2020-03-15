using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}