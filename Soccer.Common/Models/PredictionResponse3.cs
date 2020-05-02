using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Common.Models
{
    public class PredictionResponse3
    {
        public int Id { get; set; }
        public int? GoalsLocalPrediction { get; set; }
        public int? GoalsVisitorPrediction { get; set; }
        public int? GoalsLocalReal { get; set; }
        public int? GoalsVisitorReal { get; set; }
        public int? Points { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int TournamentId { get; set; }
        public string NameLocal { get; set; }
        public string NameVisitor { get; set; }
        public string LogoPathLocal { get; set; }
        public string LogoFullPathLocal => string.IsNullOrEmpty(LogoPathLocal)
           ? "noimage"//null
           : $"http://keypress.serveftp.net:88/SoccerApi{LogoPathLocal.Substring(1)}";
        public string LogoPathVisitor{ get; set; }
        public string LogoFullPathVisitor=> string.IsNullOrEmpty(LogoPathVisitor)
           ? "noimage"//null
           : $"http://keypress.serveftp.net:88/SoccerApi{LogoPathVisitor.Substring(1)}";
        public string InitialsLocal { get; set; }
        public string InitialsVisitor { get; set; }
    }
}
