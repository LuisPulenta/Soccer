using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class CloseMatchViewModel
    {
        public int MatchId { get; set; }

        public int GroupId { get; set; }
        
        public int DateNameId { get; set; }

        public int LocalId { get; set; }

        public int VisitorId { get; set; }

        [Display(Name = "GL")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int? GoalsLocal { get; set; }

        [Display(Name = "GV")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int? GoalsVisitor { get; set; }

        public GroupEntity Group { get; set; }

        public DateNameEntity DateName { get; set; }

        public TeamEntity Local { get; set; }

        public TeamEntity Visitor { get; set; }
    }
}