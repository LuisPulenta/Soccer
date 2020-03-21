using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class PredictionRequest
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int GoalsLocal { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int GoalsVisitor { get; set; }
    }
}