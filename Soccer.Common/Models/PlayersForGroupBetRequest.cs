using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class PlayersForGroupBetRequest
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int GroupBetId { get; set; }
    }
}
