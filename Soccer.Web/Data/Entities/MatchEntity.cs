using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class MatchEntity
    {
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        [Display(Name = "Fecha")]
        public DateTime DateLocal => Date.ToLocalTime();

        public TeamEntity Local { get; set; }

        public TeamEntity Visitor { get; set; }

        [Display(Name = "GL")]
        public int? GoalsLocal { get; set; }

        [Display(Name = "GV")]
        public int? GoalsVisitor { get; set; }

        [Display(Name = "Cerrado?")]
        public bool IsClosed { get; set; }

        public GroupEntity Group { get; set; }

        public DateNameEntity DateName { get; set; }
        
        public TournamentEntity Tournament { get; set; }
    }
}