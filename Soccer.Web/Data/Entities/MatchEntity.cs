using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class MatchEntity
    {
        public int Id { get; set; }

        [Display(Name = "Día y Hora")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        [Display(Name = "Día y Hora")]
        public DateTime DateLocal => Date.ToLocalTime();

        [Display(Name = "Local")]
        public TeamEntity Local { get; set; }

        [Display(Name = "Visitante")]
        public TeamEntity Visitor { get; set; }

        [Display(Name = "GL")]
        public int? GoalsLocal { get; set; }

        [Display(Name = "GV")]
        public int? GoalsVisitor { get; set; }

        [Display(Name = "Cerrado?")]
        public bool IsClosed { get; set; }

        [Display(Name = "Grupo")]
        public GroupEntity Group { get; set; }

        [Display(Name = "Fecha")]
        public DateNameEntity DateName { get; set; }
        
    }
}