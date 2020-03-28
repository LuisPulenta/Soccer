using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class GroupBetPlayer
    {
        public int Id { get; set; }

        public GroupBet GroupBet { get; set; }

        public Player Player { get; set; }

        [Display(Name = "Habilitado?")]
        public bool IsAccepted { get; set; }

        [Display(Name = "Bloqueado?")]
        public bool IsBlocked { get; set; }

        [Display(Name = "Puntos")]
        public int Points { get; set; }
    }
}
