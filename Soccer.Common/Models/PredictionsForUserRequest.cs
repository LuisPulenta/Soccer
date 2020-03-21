using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class PredictionsForUserRequest
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int TournamentId { get; set; }
    }
}